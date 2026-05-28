@echo off
setlocal EnableExtensions

set "ROOT=%~dp0"
set "DB=%ROOT%CommandEngine.dev.db"
set "SQL=%ROOT%FixSqlCatalogContextCategory.sql"

echo.
echo ============================================================
echo  Codex.CommandEngine SQL Catalog Category Fix
echo ============================================================
echo.
echo Database:
echo %DB%
echo.

if not exist "%DB%" (
    echo [ERROR] Database not found.
    echo Expected:
    echo %DB%
    exit /b 1
)

if not exist "%SQL%" (
    echo [ERROR] SQL file not found.
    echo Expected:
    echo %SQL%
    exit /b 1
)

where sqlite3 >nul 2>nul
if errorlevel 1 (
    echo [ERROR] sqlite3.exe was not found on PATH.
    echo Run the SQL manually in your SQLite tool:
    echo %SQL%
    exit /b 2
)

sqlite3 "%DB%" ".read \"%SQL%\""

if errorlevel 1 (
    echo.
    echo [FAILED] SQL catalog category update failed.
    exit /b 3
)

echo.
echo [DONE] SQL catalog category updated.
echo.
exit /b 0
