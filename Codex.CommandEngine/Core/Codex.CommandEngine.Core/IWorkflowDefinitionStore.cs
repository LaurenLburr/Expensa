namespace Codex.CommandEngine.Core;

public interface IWorkflowDefinitionStore
{
    WorkflowDefinitionDocument? FindByName(string workflowName);

    IReadOnlyList<WorkflowDefinitionDocument> ListActive();
}
