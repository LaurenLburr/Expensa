@echo off
setlocal

set DB=D:\Git\CodexExpensa\Codex.CommandEngine\DevDatabase\CommandEngine.dev.db
set SQL=D:\Git\CodexExpensa\Codex.CommandEngine\DevDatabase\Patch.CommandEngine.dev.sql

echo This patch targets:
echo   %DB%
echo.
echo If sqlite3.exe is on PATH, press any key to run it now.
echo Otherwise close this window and run Patch.CommandEngine.dev.sql in SQLiteStudio.
pause

sqlite3 "%DB%" < "%SQL%"

echo.
echo Patch complete.
pause
