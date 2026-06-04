using Codex.CommandEngine.Core;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BudgetsAddin;

public sealed class BudgetLoadRuntimeSmokeRunner
{
    private readonly SqliteBudgetRepository repository;

    public BudgetLoadRuntimeSmokeRunner()
        : this(new SqliteBudgetRepository())
    {
    }

    public BudgetLoadRuntimeSmokeRunner(
        SqliteBudgetRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);

        this.repository = repository;
    }

    public Task<CommandExecutionResult> ExecuteAsync(
        BudgetLoadRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        cancellationToken.ThrowIfCancellationRequested();

        BudgetLoadResult result =
            repository.LoadBudgets(request);

        CommandExecutionStatus status =
            string.Equals(result.Status, "Succeeded", StringComparison.OrdinalIgnoreCase)
                ? CommandExecutionStatus.Succeeded
                : CommandExecutionStatus.Failed;

        return Task.FromResult(
            new CommandExecutionResult
            {
                CorrelationId = Guid.NewGuid().ToString("N"),
                CommandName = "Budgets.LoadTree",
                Status = status,
                Message = result.Message,
                OutputJson = JsonSerializer.Serialize(
                    result,
                    BudgetJsonSerializerOptions.Default)
            });
    }
}
