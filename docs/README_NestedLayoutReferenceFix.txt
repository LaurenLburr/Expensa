Nested Layout Reference Fix

Extract into:

D:\Git\CodexExpensa

This matches the layout shown by your compiler errors:

Integration project:
D:\Git\CodexExpensa\Extensions\Host\CodexExpensa.ExtensionDevHost\CommandEngineIntegration\CodexExpensa.ExtensionDevHost.CommandEngineIntegration.csproj

Test harness project:
D:\Git\CodexExpensa\Extensions\Host\CodexExpensa.ExtensionDevHost.CommandEngineIntegration.TestHarness\CodexExpensa.ExtensionDevHost.CommandEngineIntegration.TestHarness.csproj

After extracting:
1. Close Visual Studio.
2. Run D:\Git\CodexExpensa\CleanupBuildArtifacts.bat
3. Reopen the Extensions solution.
4. Make sure these projects are loaded:
   - Codex.CommandEngine.Abstractions
   - Codex.CommandEngine.Core
   - CodexExpensa.ExtensionDevHost.CommandEngineIntegration
   - CodexExpensa.ExtensionDevHost.CommandEngineIntegration.TestHarness
5. Restore.
6. Build.

If NU1105 still appears for Codex.CommandEngine.Core:
- remove Codex.CommandEngine.Core from the solution
- add it back from:
  D:\Git\CodexExpensa\Codex.CommandEngine\Core\Codex.CommandEngine.Core\Codex.CommandEngine.Core.csproj
- restore again
