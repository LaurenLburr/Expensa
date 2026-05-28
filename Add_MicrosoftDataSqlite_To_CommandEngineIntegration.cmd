@echo off
setlocal

title Add Microsoft.Data.Sqlite to CommandEngineIntegration

echo.
echo =========================================================
echo   Add Microsoft.Data.Sqlite Package Reference
echo =========================================================
echo.

set PROJECT=D:\Git\CodexExpensa\Extensions\Host\CodexExpensa.ExtensionDevHost\CommandEngineIntegration\CodexExpensa.ExtensionDevHost.CommandEngineIntegration.csproj

if not exist "%PROJECT%" (
    echo ERROR: Project file not found:
    echo %PROJECT%
    echo.
    pause
    exit /b 1
)

echo Project:
echo %PROJECT%
echo.

dotnet add "%PROJECT%" package Microsoft.Data.Sqlite --version 10.0.5

if errorlevel 1 (
    echo.
    echo FAILED: dotnet add package failed.
    echo.
    pause
    exit /b 1
)

echo.
echo Package reference added or already present.
echo.
pause
