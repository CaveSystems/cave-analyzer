using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

namespace CleanCodeAnalyzer;

/// <summary> 
/// Analysiert Memberdeklarationen in C#‑Code und überprüft deren Namenskonventionen. 
/// </summary> 
/// <remarks> 
/// Dieser Analyzer stellt sicher, dass: 
/// <list type="bullet"> 
/// <item><description>private Member im <c>camelCase</c> benannt sind,</description></item> 
/// <item><description>öffentliche und geschützte Member im <c>PascalCase</c> benannt sind,</description></item> 
/// <item><description>keine Membernamen mit einem Unterstrich beginnen.</description></item> 
/// </list> /// Die Analyse wird für Methoden, Eigenschaften und Felder durchgeführt. 
/// Bei Verstößen werden entsprechende Diagnosen gemeldet. 
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class NamingConventionAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => new DiagnosticDescriptor[]
        {
            DiagnosticDescriptors.PrivateCamelCaseRule,
            DiagnosticDescriptors.PublicPascalCaseRule,
            DiagnosticDescriptors.NoUnderscoreRule
        }.ToImmutableArray();

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(
            AnalyzeMember,
            SyntaxKind.MethodDeclaration,
            SyntaxKind.PropertyDeclaration,
            SyntaxKind.FieldDeclaration);
    }

    static void AnalyzeMember(SyntaxNodeAnalysisContext context)
    {
        var member = (MemberDeclarationSyntax)context.Node;
        var modifiers = member.Modifiers;

        var nameToken = member switch
        {
            MethodDeclarationSyntax m => m.Identifier,
            PropertyDeclarationSyntax p => p.Identifier,
            FieldDeclarationSyntax f => f.Declaration.Variables.First().Identifier,
            _ => default
        };

        var name = nameToken.Text;

        // Global rule: no underscores
        ReportIfContainsUnderscore(context, nameToken, name);

        var isConst = modifiers.Any(SyntaxKind.ConstKeyword);
        var isPrivate = IsPrivate(modifiers);

        switch (member)
        {
            case FieldDeclarationSyntax:
                if (isConst)
                {
                    AnalyzeConstField(context, nameToken, name);
                }
                else
                {
                    AnalyzeField(context, nameToken, name, isPrivate);
                }
                break;

            case MethodDeclarationSyntax:
                AnalyzeMethod(context, nameToken, name);
                break;

            case PropertyDeclarationSyntax:
                AnalyzeProperty(context, nameToken, name, isPrivate);
                break;
        }
    }

    static bool IsPrivate(SyntaxTokenList modifiers) => modifiers.Any(SyntaxKind.PrivateKeyword) || !modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword) || m.IsKind(SyntaxKind.InternalKeyword) || m.IsKind(SyntaxKind.ProtectedKeyword));

    static void ReportIfContainsUnderscore(SyntaxNodeAnalysisContext context, SyntaxToken nameToken, string name)
    {
        if (name.Contains("_"))
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.NoUnderscoreRule, nameToken.GetLocation(), name));
        }
    }

    static void AnalyzeConstField(SyntaxNodeAnalysisContext context, SyntaxToken nameToken, string name) => ReportIfNotPascalCase(context, nameToken, name);

    static void AnalyzeMethod(SyntaxNodeAnalysisContext context, SyntaxToken nameToken, string name) => ReportIfNotPascalCase(context, nameToken, name);

    static void AnalyzeField(SyntaxNodeAnalysisContext context, SyntaxToken nameToken, string name, bool isPrivate)
    {
        if (isPrivate)
        {
            ReportIfNotCamelCase(context, nameToken, name);
        }
        else
        {
            ReportIfNotPascalCase(context, nameToken, name);
        }
    }

    static void AnalyzeProperty(SyntaxNodeAnalysisContext context, SyntaxToken nameToken, string name, bool isPrivate)
    {
        if (!isPrivate) ReportIfNotPascalCase(context, nameToken, name);
    }

    static void ReportIfNotCamelCase(SyntaxNodeAnalysisContext context, SyntaxToken nameToken, string name)
    {
        if (!IsCamelCase(name))
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.PrivateCamelCaseRule, nameToken.GetLocation(), name));
        }
    }

    static void ReportIfNotPascalCase(SyntaxNodeAnalysisContext context, SyntaxToken nameToken, string name)
    {
        if (!IsPascalCase(name))
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.PublicPascalCaseRule, nameToken.GetLocation(), name));
        }
    }

    static bool IsCamelCase(string name) => Regex.IsMatch(name, @"^[a-z][a-zA-Z0-9]*$");

    static bool IsPascalCase(string name) => Regex.IsMatch(name, @"^[A-Z][a-zA-Z0-9]*$");
}
