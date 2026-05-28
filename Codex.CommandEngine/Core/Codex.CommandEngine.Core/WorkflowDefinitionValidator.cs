namespace Codex.CommandEngine.Core;

public sealed class WorkflowDefinitionValidator : IWorkflowDefinitionValidator
{
    public WorkflowDefinitionValidationResult Validate(
        WorkflowDefinitionDocument definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        List<WorkflowDefinitionValidationIssue> issues = [];

        if (string.IsNullOrWhiteSpace(definition.WorkflowDefinitionId))
        {
            issues.Add(new WorkflowDefinitionValidationIssue
            {
                Code = "WorkflowDefinitionId.Required",
                Message = "Workflow definition id is required."
            });
        }

        if (string.IsNullOrWhiteSpace(definition.WorkflowName))
        {
            issues.Add(new WorkflowDefinitionValidationIssue
            {
                Code = "WorkflowName.Required",
                Message = "Workflow name is required."
            });
        }

        if (definition.Version <= 0)
        {
            issues.Add(new WorkflowDefinitionValidationIssue
            {
                Code = "Version.Invalid",
                Message = "Workflow version must be greater than zero."
            });
        }

        if (definition.Steps.Count == 0)
        {
            issues.Add(new WorkflowDefinitionValidationIssue
            {
                Code = "Steps.Required",
                Message = "Workflow must contain at least one step."
            });
        }

        ValidateSteps(definition.Steps, issues);

        return issues.Count == 0
            ? WorkflowDefinitionValidationResult.Success()
            : WorkflowDefinitionValidationResult.Failure(issues);
    }

    private static void ValidateSteps(
        IReadOnlyList<WorkflowDefinitionStep> steps,
        List<WorkflowDefinitionValidationIssue> issues)
    {
        HashSet<int> stepOrders = [];

        foreach (WorkflowDefinitionStep step in steps)
        {
            if (string.IsNullOrWhiteSpace(step.StepName))
            {
                issues.Add(new WorkflowDefinitionValidationIssue
                {
                    Code = "StepName.Required",
                    Message = "Workflow step name is required.",
                    StepOrder = step.StepOrder
                });
            }

            if (string.IsNullOrWhiteSpace(step.CommandName))
            {
                issues.Add(new WorkflowDefinitionValidationIssue
                {
                    Code = "CommandName.Required",
                    Message = "Workflow step command name is required.",
                    StepName = step.StepName,
                    StepOrder = step.StepOrder
                });
            }

            if (step.StepOrder <= 0)
            {
                issues.Add(new WorkflowDefinitionValidationIssue
                {
                    Code = "StepOrder.Invalid",
                    Message = "Workflow step order must be greater than zero.",
                    StepName = step.StepName,
                    StepOrder = step.StepOrder
                });
            }

            if (step.StepOrder > 0 && !stepOrders.Add(step.StepOrder))
            {
                issues.Add(new WorkflowDefinitionValidationIssue
                {
                    Code = "StepOrder.Duplicate",
                    Message = $"Workflow step order '{step.StepOrder}' is duplicated.",
                    StepName = step.StepName,
                    StepOrder = step.StepOrder
                });
            }
        }
    }
}
