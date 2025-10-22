using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace CleanCodeAnalyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class NoPrivateModifierAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(DiagnosticDescriptors.NoPrivateModifier);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration, SyntaxKind.FieldDeclaration, SyntaxKind.PropertyDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var member = (MemberDeclarationSyntax)context.Node;
        if (member.Modifiers.Any(SyntaxKind.PrivateKeyword))
        {
            var location = member.Modifiers.First(m => m.IsKind(SyntaxKind.PrivateKeyword)).GetLocation();
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.NoPrivateModifier, location));
        }
    }
}
