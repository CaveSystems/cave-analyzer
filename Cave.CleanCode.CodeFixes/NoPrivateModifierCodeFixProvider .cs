using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CleanCodeAnalyzer;

/// <summary>
/// Stellt einen Code‑Fix‑Provider bereit, der den <c>private</c>-Modifier aus 
/// Memberdeklarationen entfernt, wenn der Analyzer <see cref="DiagnosticDescriptors.NoPrivateModifier"/> 
/// einen entsprechenden Verstoß meldet. 
/// </summary> 
/// <remarks> 
/// Der Provider registriert eine Code‑Fix‑Aktion, die den <c>private</c>-Zugriffsmodifier 
/// aus der betroffenen Memberdeklaration entfernt und den Syntaxbaum entsprechend aktualisiert. 
/// </remarks>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NoPrivateModifierCodeFixProvider)), Shared]
public class NoPrivateModifierCodeFixProvider : CodeFixProvider
{
    /// <inheritdoc/>
    public override ImmutableArray<string> FixableDiagnosticIds => [DiagnosticDescriptors.NoPrivateModifier.Id];

    /// <inheritdoc/>
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <inheritdoc/>
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics.First();
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        if (root is null)
            return;

        var token = root.FindToken(diagnostic.Location.SourceSpan.Start);

        var member = token.Parent?
            .AncestorsAndSelf()
            .OfType<MemberDeclarationSyntax>()
            .FirstOrDefault();

        if (member is null)
            return;

        context.RegisterCodeFix(
            CodeAction.Create(
                title: "Remove 'private' modifier",
                createChangedDocument: c => RemovePrivateModifierAsync(context.Document, member, c),
                equivalenceKey: "RemovePrivateModifier"),
            diagnostic);
    }

    async Task<Document> RemovePrivateModifierAsync(
        Document document,
        MemberDeclarationSyntax member,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

        if (root is null)
            return document;

        var newModifiers = SyntaxFactory.TokenList(
            member.Modifiers.Where(m => !m.IsKind(SyntaxKind.PrivateKeyword)));

        var newMember = member.WithModifiers(newModifiers);

        var newRoot = root.ReplaceNode(member, newMember);

        return document.WithSyntaxRoot(newRoot);
    }
}
