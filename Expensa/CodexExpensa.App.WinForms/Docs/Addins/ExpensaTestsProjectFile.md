# Expensa Tests Project File

Adds the missing xUnit test project wrapper:

```text
Expensa\Tests\CodexExpensa.App.WinForms.Tests\CodexExpensa.App.WinForms.Tests.csproj
```

The test project targets:

```text
net8.0-windows
UseWindowsForms=true
```

and references:

```text
Expensa\CodexExpensa.App.WinForms\CodexExpensa.App.WinForms.csproj
```

## Build

From the repository root:

```powershell
dotnet test Expensa\Tests\CodexExpensa.App.WinForms.Tests\CodexExpensa.App.WinForms.Tests.csproj
```

## Solution note

This slice does not modify a `.sln` because no current Expensa solution file was provided.

If you want it visible in Visual Studio Solution Explorer, run:

```powershell
dotnet sln <your-expensa-solution.sln> add Expensa\Tests\CodexExpensa.App.WinForms.Tests\CodexExpensa.App.WinForms.Tests.csproj
```
