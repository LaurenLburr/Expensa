@echo off
setlocal

title Deploy WebsitesAddin Module

echo.
echo =========================================================
echo   Deploy WebsitesAddin Module
echo =========================================================
echo.

set ROOT=D:\Git\CodexExpensa\Extensions
set PROJECT=%ROOT%\Modules\WebsitesAddin\WebsitesAddin.csproj
set CONFIG=Debug
set TFM=net8.0
set SOURCE=%ROOT%\Modules\WebsitesAddin\bin\%CONFIG%\%TFM%
set TARGET=%ROOT%\Host\CodexExpensa.ExtensionDevHost\bin\%CONFIG%\net8.0-windows\Modules\WebsitesAddin

if not exist "%PROJECT%" (
    echo ERROR: Project file not found:
    echo %PROJECT%
    echo.
    pause
    exit /b 1
)

echo Building:
echo %PROJECT%
echo.

dotnet build "%PROJECT%" -c %CONFIG%

if errorlevel 1 (
    echo.
    echo ERROR: WebsitesAddin build failed.
    echo.
    pause
    exit /b 1
)

if not exist "%SOURCE%" (
    echo.
    echo ERROR: Build output folder not found:
    echo %SOURCE%
    echo.
    pause
    exit /b 1
)

echo.
echo Creating target folder:
echo %TARGET%
echo.

if not exist "%TARGET%" mkdir "%TARGET%"

echo Copying module output...
echo From: %SOURCE%
echo To:   %TARGET%
echo.

robocopy "%SOURCE%" "%TARGET%" /E /NFL /NDL /NJH /NJS /NP

set ROBOCOPY_EXIT=%ERRORLEVEL%

if %ROBOCOPY_EXIT% GEQ 8 (
    echo.
    echo ERROR: Robocopy failed with exit code %ROBOCOPY_EXIT%.
    echo.
    pause
    exit /b %ROBOCOPY_EXIT%
)

echo.
echo WebsitesAddin deployed successfully.
echo.
echo Target:
echo %TARGET%
echo.
pause
exit /b 0
