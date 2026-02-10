using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace CleanCodeAnalyzer;

/// <summary>
/// Analysiert Memberdeklarationen und meldet Diagnosen, wenn ein <c>private</c>-Zugriffsmodifier
/// verwendet wird.
/// </summary>
/// <remarks>
/// Dieser Analyzer überprüft Methoden, Felder und Eigenschaften und erzeugt eine Diagnose,
/// sobald ein <c>private</c>-Modifier gefunden wird.  
/// Ziel ist es, Code‑Stellen zu identifizieren, an denen der <c>private</c>-Zugriff
/// gemäß Projektkonventionen nicht erlaubt ist.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class NoPrivateModifierAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(DiagnosticDescriptors.NoPrivateModifier);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration, SyntaxKind.FieldDeclaration, SyntaxKind.PropertyDeclaration);
    }

    static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var member = (MemberDeclarationSyntax)context.Node;
        if (member.Modifiers.Any(SyntaxKind.PrivateKeyword))
        {
            var location = member.Modifiers.First(m => m.IsKind(SyntaxKind.PrivateKeyword)).GetLocation();
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.NoPrivateModifier, location));
        }
    }
}
