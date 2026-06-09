using Codex.CommandEngine.Core;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PayeesAddin;

public sealed class PayeeLoadRuntimeSmokeRunner
{
    private readonly SqlitePayeeRepository repository;

    public PayeeLoadRuntimeSmokeRunner()
        : this(new SqlitePayeeRepository())
    {
    }

    public PayeeLoadRuntimeSmokeRunner(SqlitePayeeRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        this.repository = repository;
    }

    public Task<CommandExecutionResult> ExecuteAsync(
        PayeeLoadRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        PayeeLoadResult result = repository.LoadPayees(request);

        return Task.FromResult(
            new CommandExecutionResult
            {
                CorrelationId = Guid.NewGuid().ToString("N"),
                CommandName = "Payees.LoadTree",
                Status = string.Equals(result.Status, "Succeeded", StringComparison.OrdinalIgnoreCase)
                    ? CommandExecutionStatus.Succeeded
                    : CommandExecutionStatus.Failed,
                Message = result.Message,
                OutputJson = JsonSerializer.Serialize(result, JsonOptions)
            });
    }

    private static readonly JsonSerializerOptions JsonOptions =
        new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
}
