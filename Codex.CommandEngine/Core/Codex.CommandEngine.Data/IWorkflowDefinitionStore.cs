namespace Codex.CommandEngine.Data;

public interface IWorkflowDefinitionStore
{
    WorkflowDefinitionDocument? FindByName(string workflowName);

    IReadOnlyList<WorkflowDefinitionDocument> ListActive();
}
