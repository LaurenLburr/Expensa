@echo off
setlocal EnableExtensions EnableDelayedExpansion

title Rebuild Codex.CommandEngine Dev Database

set "ROOT=%~dp0"
set "DB=%ROOT%DevDatabase\CommandEngine.dev.db"
set "SQL=%ROOT%DevDatabase\RebuildCommandEngineDatabase.sql"

echo.
echo  ============================================================
echo   Codex.CommandEngine Dev Database Rebuild
echo  ============================================================
echo.
echo   Root: %ROOT%
echo   DB:   %DB%
echo   SQL:  %SQL%
echo.

if not exist "%SQL%" (
    echo [ERROR] SQL file not found.
    echo         Expected: %SQL%
    exit /b 1
)

for /f "tokens=1-4 delims=/ " %%a in ("%date%") do set "DATESTAMP=%%d%%b%%c"
for /f "tokens=1-3 delims=:." %%a in ("%time%") do set "TIMESTAMP=%%a%%b%%c"
set "TIMESTAMP=%TIMESTAMP: =0%"
set "BACKUP=%DB%.backup.%DATESTAMP%_%TIMESTAMP%"

if exist "%DB%" (
    echo [BACKUP] Existing database found.
    echo          Copying to:
    echo          %BACKUP%
    copy /Y "%DB%" "%BACKUP%" >nul
    if errorlevel 1 (
        echo [ERROR] Backup failed. Not rebuilding.
        exit /b 1
    )
)

where sqlite3 >nul 2>nul
if errorlevel 1 (
    echo [WARNING] sqlite3.exe was not found in PATH.
    echo.
    echo The ZIP already contains a rebuilt database at:
    echo   %DB%
    echo.
    echo If you extracted this ZIP over the solution root, the template DB
    echo has already been replaced. If not, copy this file manually:
    echo   Codex.CommandEngine\DevDatabase\CommandEngine.dev.db
    echo.
    echo Install SQLite tools or add sqlite3.exe to PATH to rebuild from SQL.
    exit /b 2
)

echo.
echo [DELETE] Removing old database...
if exist "%DB%" del /F /Q "%DB%"

echo [BUILD] Rebuilding database from SQL...
sqlite3 "%DB%" ".read '%SQL%'"
if errorlevel 1 (
    echo.
    echo [ERROR] sqlite3 failed while rebuilding the database.
    echo         Backup remains here:
    echo         %BACKUP%
    exit /b 1
)

echo.
echo [VERIFY] Checking required tables...
sqlite3 "%DB%" "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name;"

echo.
echo [VERIFY] SQL catalog category counts...
sqlite3 "%DB%" "SELECT Category, COUNT(*) FROM SqlQuery GROUP BY Category ORDER BY Category;"

echo.
echo  ============================================================
echo   Rebuild complete
echo  ============================================================
echo.
echo   New DB:
echo   %DB%
echo.
echo   Backup:
echo   %BACKUP%
echo.
echo Next step:
echo   Run the integration tests.
echo.
exit /b 0
