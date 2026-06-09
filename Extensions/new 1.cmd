@echo off
setlocal

set "ROOT=D:\Git\CodexExpensa\Extensions"
set "TEST_PROJECT=%ROOT%\Tests\CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests\CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.csproj"
set "RESULTS=%ROOT%\TestResults"
set "LOGFILE=%RESULTS%\CommandEngineIntegrationTests.trx"
set "ERRORFILE=%RESULTS%\CommandEngineIntegrationTests_Output.txt"

if not exist "%RESULTS%" mkdir "%RESULTS%"

echo Running CommandEngine integration tests...
echo Test project:
echo %TEST_PROJECT%
echo.
echo Results folder:
echo %RESULTS%
echo.

dotnet test "%TEST_PROJECT%" ^
  --logger "trx;LogFileName=CommandEngineIntegrationTests.trx" ^
  --results-directory "%RESULTS%" ^
  > "%ERRORFILE%" 2>&1

echo.
echo Exit code: %ERRORLEVEL%
echo.
echo TRX file:
echo %LOGFILE%
echo.
echo Console output / errors:
echo %ERRORFILE%
echo.

pause