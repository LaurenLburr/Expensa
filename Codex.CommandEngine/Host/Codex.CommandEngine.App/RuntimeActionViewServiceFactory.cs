using Codex.CommandEngine.Core;
using Codex.CommandEngine.Data;

namespace Codex.CommandEngine.App;

public static class RuntimeActionViewServiceFactory
{
    public static RuntimeActionViewService Create(
        CommandEngineConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);

        WorkflowExecutionRepository repository =
            new(connectionFactory);

        RepositoryWorkflowRuntimeOperations operations =
            new(repository);

        WorkflowRuntimeActionService actionService =
            new(operations);

        WorkflowRuntimeActionPresenter presenter =
            new(new WorkflowRuntimeActionViewModelFactory());

        WorkflowRuntimeActionCommandService commandService =
            new(actionService, presenter);

        return new RuntimeActionViewService(commandService);
    }
}
