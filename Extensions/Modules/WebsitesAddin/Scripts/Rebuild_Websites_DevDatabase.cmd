@echo off
setlocal

title Rebuild Websites Dev Database

set ROOT=D:\Git\CodexExpensa\Extensions\Modules\WebsitesAddin
set DB_FOLDER=%ROOT%\DevDatabase
set DB_PATH=%DB_FOLDER%\websites.dev.db

echo.
echo =========================================================
echo   Rebuild Websites Dev Database
echo =========================================================
echo.

if not exist "%DB_FOLDER%" (
    echo ERROR: DevDatabase folder not found:
    echo %DB_FOLDER%
    pause
    exit /b 1
)

if exist "%DB_PATH%" (
    echo Deleting old database:
    echo %DB_PATH%
    del /f /q "%DB_PATH%"
)

where sqlite3 >nul 2>nul
if errorlevel 1 (
    echo.
    echo ERROR: sqlite3.exe was not found on PATH.
    echo Install SQLite tools or add sqlite3.exe to PATH.
    echo.
    pause
    exit /b 1
)

echo Creating:
echo %DB_PATH%
echo.

sqlite3 "%DB_PATH%" ".read %DB_FOLDER%\001_Create_Websites_Schema.sql"
if errorlevel 1 goto :failed

sqlite3 "%DB_PATH%" ".read %DB_FOLDER%\002_Seed_Websites.sql"
if errorlevel 1 goto :failed

sqlite3 "%DB_PATH%" ".read %DB_FOLDER%\003_Seed_SqlQuery.sql"
if errorlevel 1 goto :failed

echo.
echo Websites dev database rebuilt successfully.
echo.
echo %DB_PATH%
echo.
pause
exit /b 0

:failed
echo.
echo ERROR: Database rebuild failed.
echo.
pause
exit /b 1
