@echo off
setlocal EnableExtensions

set "ExtensionsRoot=D:\Git\CodexExpensa\Extensions"
set "TestProject=%ExtensionsRoot%\Tests\CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests\CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.csproj"
set "ResultsDirectory=%ExtensionsRoot%\TestResults"
set "ResultFile=CommandEngineIntegrationTests.trx"
set "ExpectedTrx=%ResultsDirectory%\%ResultFile%"
set "OutputFile=%ResultsDirectory%\CommandEngineIntegrationTests_Output.txt"

if not exist "%TestProject%" (
    echo ERROR: Test project was not found:
    echo %TestProject%
    pause
    exit /b 1
)

if exist "%ResultsDirectory%" (
    echo Clearing previous test results...
    rmdir /s /q "%ResultsDirectory%"

    if exist "%ResultsDirectory%" (
        echo ERROR: Could not clear the test-results directory:
        echo %ResultsDirectory%
        pause
        exit /b 1
    )
)

mkdir "%ResultsDirectory%"

if errorlevel 1 (
    echo ERROR: Could not create the test-results directory:
    echo %ResultsDirectory%
    pause
    exit /b 1
)

echo Running CommandEngineIntegration tests...
echo Test project: %TestProject%
echo Results directory: %ResultsDirectory%
echo.

pushd "%ExtensionsRoot%"

dotnet test "%TestProject%" ^
    --logger "trx;LogFileName=%ResultFile%" ^
    --results-directory "%ResultsDirectory%" ^
    --verbosity normal ^
    > "%OutputFile%" 2>&1

set "TestExitCode=%ERRORLEVEL%"

popd

type "%OutputFile%"

if not exist "%ExpectedTrx%" (
    for /r "%ResultsDirectory%" %%F in ("%ResultFile%") do (
        if /I not "%%~fF"=="%ExpectedTrx%" (
            copy /y "%%~fF" "%ExpectedTrx%" >nul
        )
    )
)

echo.
if exist "%ExpectedTrx%" (
    echo TRX file created:
    echo %ExpectedTrx%
) else (
    echo ERROR: No TRX file was created.
    echo.
    echo This normally means restore or compilation failed before the
    echo VSTest test host started. Review:
    echo %OutputFile%
)

echo.
echo dotnet test exit code: %TestExitCode%
echo.
pause
exit /b %TestExitCode%
