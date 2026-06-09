using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Payees;

public sealed class PayeesAddinPhase1StructureTests
{
    [Fact]
    public void PayeesAddinProject_ReferencesCommandEngineAndSqlite()
    {
        string text = ReadFile("Extensions", "Modules", "PayeesAddin", "PayeesAddin.csproj");

        Assert.Contains("Microsoft.Data.Sqlite", text);
        Assert.Contains("Codex.CommandEngine.Core", text);
        Assert.Contains("CopyLocalLockFileAssemblies", text);
    }

    [Fact]
    public void PayeeRepository_UsesPayeeTables()
    {
        string text = ReadFile("Extensions", "Modules", "PayeesAddin", "SqlitePayeeRepository.cs");

        Assert.Contains("FindPayeeTableName", text);
        Assert.Contains("'Payee'", text);
        Assert.Contains("'Payees'", text);
        Assert.Contains("pragma_table_info", text);
        Assert.Contains("LoadPayeeRows", text);
    }

    [Fact]
    public void PayeeRuntimeRunner_ReturnsCommandExecutionResult()
    {
        string text = ReadFile("Extensions", "Modules", "PayeesAddin", "PayeeLoadRuntimeSmokeRunner.cs");

        Assert.Contains("CommandExecutionResult", text);
        Assert.Contains("Payees.LoadTree", text);
        Assert.Contains("CommandExecutionStatus.Succeeded", text);
        Assert.Contains("OutputJson", text);
    }

    private static string ReadFile(params string[] parts)
    {
        string repositoryRoot = FindRepositoryRoot();
        string path = Path.Combine([repositoryRoot, .. parts]);

        Assert.True(File.Exists(path), "File was not found: " + path);

        return File.ReadAllText(path);
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, "Extensions")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException();
    }
}
