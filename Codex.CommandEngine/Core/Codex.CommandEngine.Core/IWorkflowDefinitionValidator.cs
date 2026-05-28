namespace Codex.CommandEngine.Core;

public interface IWorkflowDefinitionValidator
{
    WorkflowDefinitionValidationResult Validate(
        WorkflowDefinitionDocument definition);
}
