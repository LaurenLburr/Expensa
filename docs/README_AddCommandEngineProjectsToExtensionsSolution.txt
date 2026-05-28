Add CommandEngine Projects To Extensions Solution

Extract this zip into:

D:\Git

Run:

D:\Git\CodexExpensa\AddCommandEngineProjectsToExtensionsSolution.bat

This script tries to add these projects to the Extensions solution:

D:\Git\CodexExpensa\Codex.CommandEngine\Core\Codex.CommandEngine.Abstractions\Codex.CommandEngine.Abstractions.csproj
D:\Git\CodexExpensa\Codex.CommandEngine\Core\Codex.CommandEngine.Core\Codex.CommandEngine.Core.csproj
D:\Git\CodexExpensa\Extensions\Host\CodexExpensa.ExtensionDevHost\CommandEngineIntegration\CodexExpensa.ExtensionDevHost.CommandEngineIntegration.csproj
D:\Git\CodexExpensa\Extensions\Host\CodexExpensa.ExtensionDevHost.CommandEngineIntegration.TestHarness\CodexExpensa.ExtensionDevHost.CommandEngineIntegration.TestHarness.csproj

Then it runs:

dotnet restore

If the script cannot add projects because the solution is .slnx or Visual Studio owns the format, add the same projects manually through Visual Studio:

Solution → Add → Existing Project

Then restore/build again.
