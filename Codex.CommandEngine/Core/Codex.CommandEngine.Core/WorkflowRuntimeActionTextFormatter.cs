using System.Text;

namespace Codex.CommandEngine.Core;

public static class WorkflowRuntimeActionTextFormatter
{
    public static string Format(WorkflowRuntimeActionViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        StringBuilder builder = new();

        builder.AppendLine("Workflow Runtime Action");
        builder.AppendLine("=======================");
        builder.AppendLine();
        builder.AppendLine($"Action: {viewModel.ActionName}");
        builder.AppendLine($"Workflow Execution Id: {viewModel.WorkflowExecutionId}");
        builder.AppendLine($"Status: {viewModel.DisplayStatus}");
        builder.AppendLine($"Completed UTC: {viewModel.CompletedUtc:O}");
        builder.AppendLine();
        builder.AppendLine("Message");
        builder.AppendLine("-------");
        builder.AppendLine(viewModel.Message);

        return builder.ToString();
    }
}
