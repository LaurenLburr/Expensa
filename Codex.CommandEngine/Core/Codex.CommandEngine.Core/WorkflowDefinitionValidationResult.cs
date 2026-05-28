namespace Codex.CommandEngine.Core;

public sealed class WorkflowDefinitionValidationResult
{
    public IReadOnlyList<WorkflowDefinitionValidationIssue> Issues { get; init; } =
        [];

    public bool IsValid => Issues.Count == 0;

    public static WorkflowDefinitionValidationResult Success()
    {
        return new WorkflowDefinitionValidationResult
        {
            Issues = []
        };
    }

    public static WorkflowDefinitionValidationResult Failure(
        IReadOnlyList<WorkflowDefinitionValidationIssue> issues)
    {
        ArgumentNullException.ThrowIfNull(issues);

        return new WorkflowDefinitionValidationResult
        {
            Issues = issues
        };
    }
}
