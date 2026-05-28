@echo off
setlocal EnableExtensions EnableDelayedExpansion

title CodexExpensa Build Artifact Cleanup

set "ROOT=%~dp0"
set "LOG=%ROOT%CleanupBuildArtifacts.log"

echo.
echo ============================================================
echo  CodexExpensa Build Artifact Cleanup
echo ============================================================
echo.
echo Root:
echo   %ROOT%
echo.
echo This will remove:
echo   bin
echo   obj
echo   .vs
echo   TestResults
echo   artifacts
echo   packages\*.tmp-like leftovers only if matched below
echo.
echo Log:
echo   %LOG%
echo.

> "%LOG%" echo CodexExpensa Build Artifact Cleanup
>>"%LOG%" echo Started: %DATE% %TIME%
>>"%LOG%" echo Root: %ROOT%
>>"%LOG%" echo.

set /a FOUND=0
set /a REMOVED=0
set /a FAILED=0

echo Scanning...
echo.

for /f "delims=" %%D in ('dir "%ROOT%bin" "%ROOT%obj" "%ROOT%.vs" "%ROOT%TestResults" "%ROOT%artifacts" /ad /b /s 2^>nul') do (
    set /a FOUND+=1
    call :RemoveFolder "%%D"
)

echo.
echo ------------------------------------------------------------
echo  Cleanup Summary
echo ------------------------------------------------------------
echo  Folders found:   %FOUND%
echo  Folders removed: %REMOVED%
echo  Failed removes:  %FAILED%
echo ------------------------------------------------------------
echo.

>>"%LOG%" echo.
>>"%LOG%" echo Summary:
>>"%LOG%" echo   Folders found:   %FOUND%
>>"%LOG%" echo   Folders removed: %REMOVED%
>>"%LOG%" echo   Failed removes:  %FAILED%
>>"%LOG%" echo Finished: %DATE% %TIME%

if %FAILED% GTR 0 (
    echo Some folders could not be removed.
    echo Close Visual Studio, terminals, test runners, and try again.
    echo See log:
    echo   %LOG%
    echo.
    pause
    exit /b 1
)

echo Cleanup complete.
echo.
pause
exit /b 0

:RemoveFolder
set "TARGET=%~1"

echo [FOUND] %TARGET%
>>"%LOG%" echo [FOUND] %TARGET%

if not exist "%TARGET%" (
    echo   [SKIP] Already gone.
    >>"%LOG%" echo   [SKIP] Already gone.
    goto :eof
)

echo   Removing...
rmdir /s /q "%TARGET%" 2>nul

if exist "%TARGET%" (
    echo   [FAIL] Could not remove.
    >>"%LOG%" echo   [FAIL] Could not remove.
    set /a FAILED+=1
) else (
    echo   [OK] Removed.
    >>"%LOG%" echo   [OK] Removed.
    set /a REMOVED+=1
)

goto :eof
