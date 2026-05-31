@echo off
setlocal

title Copy Expensa Prod To Websites Dev Database

set SCRIPT_FOLDER=%~dp0

powershell -NoProfile -ExecutionPolicy Bypass -File "%SCRIPT_FOLDER%Copy_Expensa_Prod_To_Websites_DevDatabase.ps1"

if errorlevel 1 (
    echo.
    echo FAILED: Copy Expensa production data to Websites dev database failed.
    echo.
    pause
    exit /b 1
)

echo.
echo Done.
echo.
pause
