using Codex.CommandEngine.Core;
using Codex.CommandEngine.Data;

namespace Codex.CommandEngine.App;

public static class RuntimeOperationsViewServiceFactory
{
    public static RuntimeOperationsViewService CreateEmpty()
    {
        return new RuntimeOperationsViewService(
            new RuntimeOperationsNullService(),
            new WorkflowHeartbeatEvaluator(),
            TimeSpan.FromMinutes(10));
    }

    public static RuntimeOperationsViewService Create(
        IWorkflowRuntimeOperations operations,
        TimeSpan staleAfter)
    {
        ArgumentNullException.ThrowIfNull(operations);

        return new RuntimeOperationsViewService(
            operations,
            new WorkflowHeartbeatEvaluator(),
            staleAfter);
    }

    public static RuntimeOperationsViewService CreateFromConnectionFactory(
        CommandEngineConnectionFactory connectionFactory,
        TimeSpan staleAfter)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);

        WorkflowExecutionRepository repository =
            new(connectionFactory);

        RepositoryWorkflowRuntimeOperations operations =
            new(repository);

        return Create(operations, staleAfter);
    }
}
