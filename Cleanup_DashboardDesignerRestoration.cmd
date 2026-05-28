@echo off
setlocal

title Cleanup Dashboard Runtime-Built Form Partials

echo.
echo =========================================================
echo   Dashboard Designer Restoration Cleanup
echo =========================================================
echo.

set ROOT=D:\Git\CodexExpensa\Extensions\Host\CodexExpensa.ExtensionDevHost\CommandEngineIntegration

if not exist "%ROOT%" (
    echo ERROR: Folder not found:
    echo %ROOT%
    echo.
    pause
    exit /b 1
)

call :DeleteFile "ExtensionRuntimeDashboardForm.DuplicateToggle.cs"
call :DeleteFile "ExtensionRuntimeDashboardForm.DuplicateHelpers.cs"
call :DeleteFile "ExtensionRuntimeDashboardForm.ExecutionResult.cs"
call :DeleteFile "ExtensionRuntimeDashboardForm.Parameters.cs"
call :DeleteFile "ExtensionRuntimeDashboardController.Parameters.cs"

echo.
echo Cleanup complete.
echo.
pause
exit /b 0

:DeleteFile
set FILE=%~1

if exist "%ROOT%\%FILE%" (
    del /f /q "%ROOT%\%FILE%"
    if exist "%ROOT%\%FILE%" (
        echo FAILED  - %FILE%
    ) else (
        echo DELETED - %FILE%
    )
) else (
    echo MISSING - %FILE%
)

exit /b
