using CodexExpensa.ExtensionDevHost.CommandEngineIntegration;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests;

public sealed class CommandExecutionPersistentRecordQueryBuilderTests
{
    [Fact]
    public void Build_WhenQueueSource_AddsQueueFilter()
    {
        CommandExecutionPersistentRecordQuerySql sql =
            CommandExecutionPersistentRecordQueryBuilder.Build(
                new CommandExecutionPersistentRecordQuery
                {
                    SourceFilter = CommandExecutionPersistentRecordSourceFilter.Queue
                });

        Assert.Contains("IsQueueItem = 1", sql.SqlText);
    }

    [Fact]
    public void Build_WhenSearchText_AddsLikeParameter()
    {
        CommandExecutionPersistentRecordQuerySql sql =
            CommandExecutionPersistentRecordQueryBuilder.Build(
                new CommandExecutionPersistentRecordQuery
                {
                    SearchText = "bank",
                    MaximumRows = 25
                });

        Assert.Contains("LIKE @SearchText", sql.SqlText);
        Assert.Equal("%bank%", sql.Parameters["@SearchText"]);
        Assert.Equal(25, sql.Parameters["@MaximumRows"]);
    }

    [Fact]
    public void Build_WhenFailedFirst_OrdersFailedFirst()
    {
        CommandExecutionPersistentRecordQuerySql sql =
            CommandExecutionPersistentRecordQueryBuilder.Build(
                new CommandExecutionPersistentRecordQuery
                {
                    SortMode = CommandExecutionPersistentRecordSortMode.FailedFirst
                });

        Assert.Contains("Status = 'Failed'", sql.SqlText);
    }
}
