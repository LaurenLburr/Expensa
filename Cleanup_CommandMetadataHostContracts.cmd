@echo off
setlocal

title Cleanup Host Command Metadata Contracts

echo.
echo =========================================================
echo   Cleanup misplaced Command Metadata host contracts
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

call :DeleteFile "CommandMetadataRecord.cs"
call :DeleteFile "ICommandMetadataProvider.cs"

echo.
echo Done.
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
