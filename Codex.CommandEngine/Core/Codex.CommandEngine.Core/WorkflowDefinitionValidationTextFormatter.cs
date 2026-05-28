using System.Text;

namespace Codex.CommandEngine.Core;

public static class WorkflowDefinitionValidationTextFormatter
{
    public static string Format(
        WorkflowDefinitionDocument definition,
        WorkflowDefinitionValidationResult result)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(result);

        StringBuilder builder = new();

        builder.AppendLine("Workflow Definition Validation");
        builder.AppendLine("==============================");
        builder.AppendLine();
        builder.AppendLine($"Workflow: {definition.WorkflowName}");
        builder.AppendLine($"Definition Id: {definition.WorkflowDefinitionId}");
        builder.AppendLine($"Version: {definition.Version}");
        builder.AppendLine($"Is Valid: {result.IsValid}");
        builder.AppendLine();

        if (result.IsValid)
        {
            builder.AppendLine("No validation issues found.");
            return builder.ToString();
        }

        builder.AppendLine("Issues");
        builder.AppendLine("------");

        foreach (WorkflowDefinitionValidationIssue issue in result.Issues)
        {
            builder.AppendLine();
            builder.AppendLine($"Code: {issue.Code}");
            builder.AppendLine($"Message: {issue.Message}");

            if (!string.IsNullOrWhiteSpace(issue.StepName))
            {
                builder.AppendLine($"Step: {issue.StepName}");
            }

            if (issue.StepOrder.HasValue)
            {
                builder.AppendLine($"Step Order: {issue.StepOrder.Value}");
            }
        }

        return builder.ToString();
    }
}
