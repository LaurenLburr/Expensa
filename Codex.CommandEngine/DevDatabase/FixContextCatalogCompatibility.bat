@echo off
setlocal EnableExtensions

set "ROOT=%~dp0"
set "DB=%ROOT%CommandEngine.dev.db"
set "SQL=%ROOT%FixContextCatalogCompatibility.sql"

echo.
echo ============================================================
echo  Codex.CommandEngine Context Catalog Compatibility Fix
echo ============================================================
echo.

if not exist "%DB%" (
    echo [ERROR] Database not found:
    echo %DB%
    exit /b 1
)

if not exist "%SQL%" (
    echo [ERROR] SQL file not found:
    echo %SQL%
    exit /b 1
)

where sqlite3 >nul 2>nul
if errorlevel 1 (
    echo [ERROR] sqlite3.exe was not found on PATH.
    echo Run this SQL manually instead:
    echo %SQL%
    exit /b 2
)

sqlite3 "%DB%" ".read \"%SQL%\""

if errorlevel 1 (
    echo [FAILED] Context catalog compatibility update failed.
    exit /b 3
)

echo [DONE] Context catalog compatibility rows updated.
exit /b 0
