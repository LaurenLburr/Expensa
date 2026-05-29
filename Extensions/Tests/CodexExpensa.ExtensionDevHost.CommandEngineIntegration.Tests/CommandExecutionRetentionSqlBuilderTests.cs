using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class CommandExecutionRetentionSqlBuilderTests
{
    [Fact]
    public void BuildDeleteSql_WhenFailedStatus_AddsStatusFilter()
    {
        CommandExecutionPersistentRecordQuerySql sql =
            CommandExecutionRetentionSqlBuilder.BuildDeleteSql(
                new CommandExecutionRetentionOptions
                {
                    Status = "Failed"
                });

        Assert.Contains("DELETE FROM CommandExecution", sql.SqlText);
        Assert.Contains("Status = @Status", sql.SqlText);
        Assert.Equal("Failed", sql.Parameters["@Status"]);
    }

    [Fact]
    public void BuildDeleteSql_WhenHistorySource_AddsHistoryFilter()
    {
        CommandExecutionPersistentRecordQuerySql sql =
            CommandExecutionRetentionSqlBuilder.BuildDeleteSql(
                new CommandExecutionRetentionOptions
                {
                    SourceFilter = CommandExecutionPersistentRecordSourceFilter.History
                });

        Assert.Contains("IsHistory = 1", sql.SqlText);
    }
}
