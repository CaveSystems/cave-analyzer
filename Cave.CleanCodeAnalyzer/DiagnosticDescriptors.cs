using Microsoft.CodeAnalysis;

namespace CleanCodeAnalyzer;

public static class DiagnosticDescriptors
{
    public static readonly DiagnosticDescriptor NoPrivateModifier = new DiagnosticDescriptor(
        id: "CC0001",
        title: "Verwendung von 'private' ist nicht erlaubt",
        messageFormat: "Der Zugriffsmodifizierer 'private' sollte nicht verwendet werden",
        category: DiagnosticCategories.Design,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);


    public static readonly DiagnosticDescriptor PrivateCamelCaseRule = new(
        id: "CC0002",
        title: "Private Member müssen camelCase sein",
        messageFormat: "Der private Member '{0}' sollte in camelCase geschrieben sein",
        category: DiagnosticCategories.Naming,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor PublicPascalCaseRule = new(
        id: "CC0003",
        title: "Nicht-private Member müssen PascalCase sein",
        messageFormat: "Der Member '{0}' sollte in PascalCase geschrieben sein",
        category: DiagnosticCategories.Naming,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor NoUnderscoreRule = new(
        id: "CC0004",
        title: "Unterstriche in Namen sind nicht erlaubt",
        messageFormat: "Der Name '{0}' enthält einen Unterstrich und sollte umbenannt werden",
        category: DiagnosticCategories.Naming,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);
}
