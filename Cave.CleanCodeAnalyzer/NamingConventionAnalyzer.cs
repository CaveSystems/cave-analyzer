using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

namespace CleanCodeAnalyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class NamingConventionAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => new DiagnosticDescriptor[] 
    {
        DiagnosticDescriptors.PrivateCamelCaseRule,
        DiagnosticDescriptors.PublicPascalCaseRule,
        DiagnosticDescriptors.NoUnderscoreRule
    }.ToImmutableArray();

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeMember, SyntaxKind.MethodDeclaration, SyntaxKind.PropertyDeclaration, SyntaxKind.FieldDeclaration);
    }

    private static void AnalyzeMember(SyntaxNodeAnalysisContext context)
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
        var isPrivate = modifiers.Any(SyntaxKind.PrivateKeyword) || (!modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword) || m.IsKind(SyntaxKind.InternalKeyword) || m.IsKind(SyntaxKind.ProtectedKeyword)));

        if (isPrivate && !IsCamelCase(name))
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.PrivateCamelCaseRule, nameToken.GetLocation(), name));
        }
        else if (!isPrivate && !IsPascalCase(name))
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.PublicPascalCaseRule, nameToken.GetLocation(), name));
        }
    }

    private static bool IsCamelCase(string name) =>
        Regex.IsMatch(name, @"^[a-z][a-zA-Z0-9]*$");

    private static bool IsPascalCase(string name) =>
        Regex.IsMatch(name, @"^[A-Z][a-zA-Z0-9]*$");
}
