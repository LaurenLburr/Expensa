@echo off
setlocal EnableExtensions EnableDelayedExpansion

title Codex.CommandEngine Build Folder Cleanup

set "ROOT=%~dp0"
set "ROOT=%ROOT:~0,-1%"

echo.
echo  ============================================================
echo   Codex.CommandEngine Build Folder Cleanup
echo  ============================================================
echo.
echo   Root:
echo   %ROOT%
echo.
echo   This will remove build output folders under this solution:
echo     - bin
echo     - obj
echo     - Debug
echo     - Release
echo.
echo   It will NOT remove:
echo     - .git
echo     - .vs
echo     - Upload
echo     - DevDatabase
echo     - source files
echo.
echo  ============================================================
echo.

if not exist "%ROOT%\Codex.CommandEngine.sln" (
    if not exist "%ROOT%\Codex.CommandEngine.slnx" (
        echo [WARNING] Could not find Codex.CommandEngine.sln or Codex.CommandEngine.slnx in:
        echo           %ROOT%
        echo.
        echo This script is intended to live in:
        echo   D:\Git\CodexExpensa\Codex.CommandEngine
        echo.
        choice /C YN /N /M "Continue anyway? [Y/N]: "
        if errorlevel 2 (
            echo.
            echo Cancelled. No folders were removed.
            exit /b 1
        )
    )
)

echo Scanning for build folders...
echo.

set /a FOUND=0
set /a DELETED=0
set /a FAILED=0

set "LIST_FILE=%TEMP%\codex_commandengine_cleanup_%RANDOM%%RANDOM%.txt"

if exist "%LIST_FILE%" del "%LIST_FILE%" >nul 2>nul

for /d /r "%ROOT%" %%D in (bin obj Debug Release) do (
    set "TARGET=%%D"

    echo !TARGET! | findstr /I "\\.git\\" >nul
    if errorlevel 1 (
        echo !TARGET! | findstr /I "\\.vs\\" >nul
        if errorlevel 1 (
            echo !TARGET! | findstr /I "\\Upload\\" >nul
            if errorlevel 1 (
                echo !TARGET! | findstr /I "\\DevDatabase\\" >nul
                if errorlevel 1 (
                    if exist "!TARGET!" (
                        echo !TARGET!>>"%LIST_FILE%"
                        set /a FOUND+=1
                    )
                )
            )
        )
    )
)

if %FOUND% EQU 0 (
    echo [OK] No build folders found. Already clean. Fancy that.
    echo.
    if exist "%LIST_FILE%" del "%LIST_FILE%" >nul 2>nul
    exit /b 0
)

echo Found %FOUND% folder(s) to remove:
echo.

for /f "usebackq delims=" %%D in ("%LIST_FILE%") do (
    echo   %%D
)

echo.
choice /C YN /N /M "Delete these folders? [Y/N]: "
if errorlevel 2 (
    echo.
    echo Cancelled. No folders were removed.
    if exist "%LIST_FILE%" del "%LIST_FILE%" >nul 2>nul
    exit /b 1
)

echo.
echo  ============================================================
echo   Deleting build folders
echo  ============================================================
echo.

for /f "usebackq delims=" %%D in ("%LIST_FILE%") do (
    echo [DELETE] %%D
    rmdir /s /q "%%D" >nul 2>nul

    if exist "%%D" (
        echo   [FAILED] Could not delete.
        set /a FAILED+=1
    ) else (
        echo   [OK] Removed.
        set /a DELETED+=1
    )
)

if exist "%LIST_FILE%" del "%LIST_FILE%" >nul 2>nul

echo.
echo  ============================================================
echo   Cleanup Summary
echo  ============================================================
echo.
echo   Found:    %FOUND%
echo   Removed:  %DELETED%
echo   Failed:   %FAILED%
echo.

if %FAILED% GTR 0 (
    echo [WARNING] Some folders could not be removed.
    echo           Close Visual Studio, terminals, test runners, or file explorers
    echo           that may be holding files open, then run this again.
    echo.
    exit /b 2
)

echo [DONE] Build folders removed successfully.
echo.
echo Suggested next steps:
echo   1. Reopen Visual Studio if needed.
echo   2. Rebuild the solution.
echo   3. If NuGet gets cranky, restore packages.
echo.

exit /b 0
