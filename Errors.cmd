@echo off
setlocal EnableExtensions EnableDelayedExpansion

rem ============================================================================
rem Generic .NET test runner
rem
rem Usage:
rem   Errors.cmd
rem       Runs every *.Tests.csproj beneath the folder containing this script.
rem
rem   Errors.cmd "D:\Git\CodexExpensa\Expensa"
rem       Runs every *.Tests.csproj beneath the supplied folder.
rem
rem   Errors.cmd "D:\Git\CodexExpensa\Expensa\Tests\My.Tests\My.Tests.csproj"
rem       Runs one specific test project.
rem ============================================================================

set "ScriptRoot=%~dp0"
set "Target=%~1"

if not defined Target (
    set "Target=%ScriptRoot%"
)

for %%I in ("%Target%") do set "Target=%%~fI"

if not exist "%Target%" (
    echo ERROR: The requested test target was not found:
    echo %Target%
    echo.
    pause
    exit /b 1
)

if /I "%~x1"==".csproj" (
    set "SearchRoot=%~dp1"
    set "SingleProject=%Target%"
) else (
    if /I "%~x1"==".sln" (
        set "SearchRoot=%~dp1"
        set "SingleProject="
        set "SolutionTarget=%Target%"
    ) else (
        set "SearchRoot=%Target%"
        set "SingleProject="
        set "SolutionTarget="
    )
)

for %%I in ("%SearchRoot%") do set "SearchRoot=%%~fI"

set "ResultsDirectory=%SearchRoot%\TestResults"
set "SummaryFile=%ResultsDirectory%\TestSummary.txt"

echo Generic .NET test runner
echo ========================
echo Target: %Target%
echo Results: %ResultsDirectory%
echo.

if exist "%ResultsDirectory%" (
    echo Clearing previous test results...
    rmdir /s /q "%ResultsDirectory%"

    if exist "%ResultsDirectory%" (
        echo ERROR: Could not clear the test-results directory:
        echo %ResultsDirectory%
        echo.
        pause
        exit /b 1
    )
)

mkdir "%ResultsDirectory%"

if errorlevel 1 (
    echo ERROR: Could not create the test-results directory:
    echo %ResultsDirectory%
    echo.
    pause
    exit /b 1
)

(
    echo Generic .NET test run
    echo =====================
    echo Started: %DATE% %TIME%
    echo Target: %Target%
    echo.
) > "%SummaryFile%"

set /a ProjectCount=0
set /a PassedProjectCount=0
set /a FailedProjectCount=0
set "OverallExitCode=0"

if defined SingleProject (
    call :RunProject "%SingleProject%"
    goto AfterProjects
)

if defined SolutionTarget (
    call :RunSolution "%SolutionTarget%"
    goto AfterProjects
)

for /r "%SearchRoot%" %%F in (*.Tests.csproj) do (
    set "Candidate=%%~fF"

    echo !Candidate! | findstr /I /C:"\bin\" /C:"\obj\" >nul
    if errorlevel 1 (
        call :RunProject "!Candidate!"
    )
)

:AfterProjects

if %ProjectCount% EQU 0 (
    echo ERROR: No test projects were found.
    echo.
    echo Search root:
    echo %SearchRoot%
    echo.
    echo Expected project names matching:
    echo *.Tests.csproj
    echo.
    >> "%SummaryFile%" echo No test projects were found.
    pause
    exit /b 1
)

(
    echo.
    echo Summary
    echo -------
    echo Projects run: %ProjectCount%
    echo Projects passed: %PassedProjectCount%
    echo Projects failed: %FailedProjectCount%
    echo Completed: %DATE% %TIME%
    echo Overall exit code: %OverallExitCode%
) >> "%SummaryFile%"

echo.
echo ============================================================
echo Test run complete
echo ============================================================
echo Projects run:    %ProjectCount%
echo Projects passed: %PassedProjectCount%
echo Projects failed: %FailedProjectCount%
echo.
echo Summary:
echo %SummaryFile%
echo.

if %OverallExitCode% NEQ 0 (
    echo One or more test projects failed.
) else (
    echo All test projects passed.
)

echo.
pause
exit /b %OverallExitCode%


:RunProject
set "TestProject=%~1"

for %%I in ("%TestProject%") do (
    set "ProjectName=%%~nI"
    set "ProjectFolder=%%~dpI"
)

set /a ProjectCount+=1

set "ProjectResults=%ResultsDirectory%\%ProjectName%"
set "ResultFile=%ProjectName%.trx"
set "ExpectedTrx=%ProjectResults%\%ResultFile%"
set "OutputFile=%ProjectResults%\%ProjectName%_Output.txt"

mkdir "%ProjectResults%" >nul 2>&1

echo.
echo ------------------------------------------------------------
echo Running: %ProjectName%
echo Project: %TestProject%
echo ------------------------------------------------------------

dotnet test "%TestProject%" ^
    --logger "trx;LogFileName=%ResultFile%" ^
    --results-directory "%ProjectResults%" ^
    --verbosity normal ^
    > "%OutputFile%" 2>&1

set "ProjectExitCode=%ERRORLEVEL%"

type "%OutputFile%"

if not exist "%ExpectedTrx%" (
    for /r "%ProjectResults%" %%T in ("%ResultFile%") do (
        if /I not "%%~fT"=="%ExpectedTrx%" (
            copy /y "%%~fT" "%ExpectedTrx%" >nul
        )
    )
)

if "%ProjectExitCode%"=="0" (
    set /a PassedProjectCount+=1
    set "ProjectStatus=PASSED"
) else (
    set /a FailedProjectCount+=1
    set "ProjectStatus=FAILED"
    set "OverallExitCode=1"
)

(
    echo [%ProjectStatus%] %ProjectName%
    echo   Project: %TestProject%
    echo   Exit code: %ProjectExitCode%
    echo   Output: %OutputFile%
    if exist "%ExpectedTrx%" (
        echo   TRX: %ExpectedTrx%
    ) else (
        echo   TRX: not created
    )
    echo.
) >> "%SummaryFile%"

if exist "%ExpectedTrx%" (
    echo.
    echo TRX file:
    echo %ExpectedTrx%
) else (
    echo.
    echo WARNING: No TRX file was created for %ProjectName%.
    echo This usually means restore or compilation failed before VSTest started.
    echo Review:
    echo %OutputFile%
)

exit /b 0


:RunSolution
set "Solution=%~1"

for %%I in ("%Solution%") do set "SolutionName=%%~nI"

set /a ProjectCount+=1

set "ProjectResults=%ResultsDirectory%\%SolutionName%"
set "ResultFile=%SolutionName%.trx"
set "ExpectedTrx=%ProjectResults%\%ResultFile%"
set "OutputFile=%ProjectResults%\%SolutionName%_Output.txt"

mkdir "%ProjectResults%" >nul 2>&1

echo.
echo ------------------------------------------------------------
echo Running solution: %SolutionName%
echo Solution: %Solution%
echo ------------------------------------------------------------

dotnet test "%Solution%" ^
    --logger "trx;LogFileName=%ResultFile%" ^
    --results-directory "%ProjectResults%" ^
    --verbosity normal ^
    > "%OutputFile%" 2>&1

set "ProjectExitCode=%ERRORLEVEL%"

type "%OutputFile%"

if "%ProjectExitCode%"=="0" (
    set /a PassedProjectCount+=1
    set "ProjectStatus=PASSED"
) else (
    set /a FailedProjectCount+=1
    set "ProjectStatus=FAILED"
    set "OverallExitCode=1"
)

(
    echo [%ProjectStatus%] %SolutionName%
    echo   Solution: %Solution%
    echo   Exit code: %ProjectExitCode%
    echo   Output: %OutputFile%
    echo   Results folder: %ProjectResults%
    echo.
) >> "%SummaryFile%"

exit /b 0
