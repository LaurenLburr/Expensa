@echo off
setlocal EnableDelayedExpansion

title CodexExpensa Build Cleanup

cd /d "%~dp0"

echo.
echo =========================================================
echo           CODEXEXPENSA BUILD CLEANUP
echo =========================================================
echo.
echo Root:
echo %CD%
echo.

set /a BINCOUNT=0
set /a OBJCOUNT=0
set /a VSCOUNT=0
set /a TESTCOUNT=0
set /a ERRORCOUNT=0

echo Scanning folders...
echo.

REM =========================================================
REM DELETE BIN
REM =========================================================

for /d /r %%D in (bin) do (
    if exist "%%D" (
        echo [BIN ] Deleting: %%D
        rmdir /s /q "%%D"

        if exist "%%D" (
            echo        FAILED
            set /a ERRORCOUNT+=1
        ) else (
            set /a BINCOUNT+=1
        )
    )
)

echo.

REM =========================================================
REM DELETE OBJ
REM =========================================================

for /d /r %%D in (obj) do (
    if exist "%%D" (
        echo [OBJ ] Deleting: %%D
        rmdir /s /q "%%D"

        if exist "%%D" (
            echo        FAILED
            set /a ERRORCOUNT+=1
        ) else (
            set /a OBJCOUNT+=1
        )
    )
)

echo.

REM =========================================================
REM DELETE TESTRESULTS
REM =========================================================

for /d /r %%D in (TestResults) do (
    if exist "%%D" (
        echo [TEST] Deleting: %%D
        rmdir /s /q "%%D"

        if exist "%%D" (
            echo        FAILED
            set /a ERRORCOUNT+=1
        ) else (
            set /a TESTCOUNT+=1
        )
    )
)

echo.

REM =========================================================
REM DELETE .VS
REM =========================================================

for /d /r %%D in (.vs) do (
    if exist "%%D" (
        echo [ .VS] Deleting: %%D
        attrib -h "%%D" >nul 2>&1

        rmdir /s /q "%%D"

        if exist "%%D" (
            echo        FAILED
            set /a ERRORCOUNT+=1
        ) else (
            set /a VSCOUNT+=1
        )
    )
)

echo.
echo =========================================================
echo                    CLEANUP COMPLETE
echo =========================================================
echo.
echo Bin folders deleted       : %BINCOUNT%
echo Obj folders deleted       : %OBJCOUNT%
echo TestResults deleted       : %TESTCOUNT%
echo .vs folders deleted       : %VSCOUNT%
echo Errors                    : %ERRORCOUNT%
echo.

if %ERRORCOUNT% GTR 0 (
    echo Some folders could not be deleted.
    echo Usually this means:
    echo.
    echo   - Visual Studio is still open
    echo   - testhost.exe is still running
    echo   - dotnet.exe is still holding files
    echo.
    echo Translation:
    echo "Windows is emotionally attached to your DLLs."
)

echo.
pause