CodexExpensa cleanup scripts

Extract this zip into:

D:\Git

Expected files:

D:\Git\CodexExpensa\CleanupBuildArtifacts.bat
D:\Git\CodexExpensa\CleanupBuildArtifacts.ps1

Run from:

D:\Git\CodexExpensa

Recommended use:

1. Close Visual Studio.
2. Run CleanupBuildArtifacts.bat.
3. Reopen Visual Studio.
4. Restore NuGet packages.
5. Rebuild solution.

The BAT removes all folders named:
- bin
- obj
- .vs
- TestResults
- artifacts

It writes a log file:

D:\Git\CodexExpensa\CleanupBuildArtifacts.log
