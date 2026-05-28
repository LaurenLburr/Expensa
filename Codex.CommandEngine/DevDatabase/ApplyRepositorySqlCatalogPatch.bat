@echo off
setlocal

set DB=D:\Git\CodexExpensa\Codex.CommandEngine\DevDatabase\CommandEngine.dev.db
set SQL=D:\Git\CodexExpensa\Codex.CommandEngine\DevDatabase\Patch.RepositorySqlCatalog.sql

echo Applying repository SQL catalog patch to:
echo   %DB%
echo.
echo If sqlite3.exe is on PATH, press any key to run it now.
echo Otherwise close this window and run Patch.RepositorySqlCatalog.sql in SQLiteStudio.
pause

sqlite3 "%DB%" < "%SQL%"

echo.
echo Patch complete.
pause
