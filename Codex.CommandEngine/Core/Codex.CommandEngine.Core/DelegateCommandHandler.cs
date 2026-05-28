using AbstractionsCommandDefinition = Codex.CommandEngine.Abstractions.CommandDefinition;
using AbstractionsCommandExecutionResult = Codex.CommandEngine.Abstractions.CommandExecutionResult;
using AbstractionsCommandRequest = Codex.CommandEngine.Abstractions.CommandRequest;
using AbstractionsICommandHandler = Codex.CommandEngine.Abstractions.ICommandHandler;

namespace Codex.CommandEngine.Core;

public sealed class DelegateCommandHandler : AbstractionsICommandHandler
{
    private readonly Func<AbstractionsCommandRequest, CancellationToken, Task<AbstractionsCommandExecutionResult>> _execute;
    private readonly Func<AbstractionsCommandRequest, CancellationToken, Task<AbstractionsCommandExecutionResult>>? _validate;

    public DelegateCommandHandler(
        AbstractionsCommandDefinition definition,
        Func<AbstractionsCommandRequest, CancellationToken, Task<AbstractionsCommandExecutionResult>> execute,
        Func<AbstractionsCommandRequest, CancellationToken, Task<AbstractionsCommandExecutionResult>>? validate = null)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(execute);
        ArgumentException.ThrowIfNullOrWhiteSpace(definition.CommandName);
        ArgumentException.ThrowIfNullOrWhiteSpace(definition.DisplayName);

        Definition = definition;
        _execute = execute;
        _validate = validate;
    }

    public AbstractionsCommandDefinition Definition { get; }

    public Task<AbstractionsCommandExecutionResult> ValidateAsync(
        AbstractionsCommandRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (_validate is not null)
        {
            return _validate(request, cancellationToken);
        }

        return Task.FromResult(AbstractionsCommandExecutionResult.Success("Validation succeeded."));
    }

    public async Task<AbstractionsCommandExecutionResult> ExecuteAsync(
        AbstractionsCommandRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        AbstractionsCommandExecutionResult validationResult =
            await ValidateAsync(request, cancellationToken).ConfigureAwait(false);

        if (!validationResult.Succeeded)
        {
            return validationResult;
        }

        return await _execute(request, cancellationToken).ConfigureAwait(false);
    }
}
