# Cave Clean Code Analyzer

Der Cave Clean Code Analyzer ist ein Roslyn basierter Analyzer, der typische Clean Code Regeln automatisch prüft und Verstöße direkt im Editor oder während des Builds meldet.

Das Projekt besteht aus drei Komponenten:

- **Cave.CleanCode.Analyzer** – enthält alle Roslyn Analyzer  
- **Cave.CleanCode.CodeFixes** – enthält die zugehörigen CodeFixProvider  
- **Cave.CleanCode.Package** – erzeugt das NuGet Paket `Cave.CleanCodeAnalyzer`

---

## Features

### NoPrivateModifierAnalyzer
Erkennt unnötige `private` Zugriffsmodifizierer und meldet sie als Diagnose.  
Ein verfügbarer CodeFix entfernt automatisch alle `private` Modifier.

Beispiel:

```
private int value;   // Meldung: Private modifier should be omitted
```

---

### NamingConventionAnalyzer
Überprüft, ob Member Namen den definierten Namenskonventionen entsprechen.

Erkannte Verstöße:

- Private Member müssen camelCase verwenden  
- Öffentliche Member müssen PascalCase verwenden  
- Unterstriche im Namen sind nicht erlaubt

Beispiele:

```
private int MyValue;        // Private member names must be camelCase
public string myProperty;   // Public member names must be PascalCase
int _counter;               // Underscores are not allowed
```

Unterstützte Membertypen:

- Methoden  
- Properties  
- Felder  

---

## Installation

Das Paket ist über NuGet verfügbar.

Package Manager:

```
Install-Package Cave.CleanCodeAnalyzer
```

.NET CLI:

```
dotnet add package Cave.CleanCodeAnalyzer
```

Nach der Installation werden Analyzer und CodeFixes automatisch aktiviert – sowohl im Editor als auch im Build.

---

## Projektstruktur

```
/Cave.CleanCode.Analyzer
    Enthält alle Roslyn-Diagnose-Analyzer

/Cave.CleanCode.CodeFixes
    Enthält alle CodeFixProvider für automatische Korrekturen

/Cave.CleanCode.Package
    Erzeugt das NuGet-Paket (Analyzer + CodeFixes)
```

Das NuGet Paket enthält beide Assemblies unter:

```
/analyzers/dotnet/cs/
```

Damit werden Analyzer und CodeFixes automatisch von Visual Studio, Rider und MSBuild geladen.

---

## Entwicklung

### Voraussetzungen

- .NET 8 SDK oder neuer  
- Visual Studio 2022 oder JetBrains Rider

### Build

```
dotnet build
```

### NuGet Paket erzeugen

Im Packaging Projekt:

```
dotnet pack -c Release
```

Erzeugt:

- `Cave.CleanCodeAnalyzer.<version>.nupkg`  
- `Cave.CleanCodeAnalyzer.<version>.snupkg`

---

## Beiträge

Beiträge sind willkommen.

Typische Beiträge:

- neue Analyzer  
- neue CodeFixes  
- Verbesserungen bestehender Regeln  
- Bugfixes  
- Dokumentation  

Bitte vor größeren Änderungen ein Issue eröffnen, um die Idee zu besprechen.

---

## Lizenz

Dieses Projekt steht unter der MIT Lizenz. Details siehe Datei `LICENSE`.
