namespace Codex.CommandEngine.Core;

public sealed class WorkflowRuntimeActionPresenter : IWorkflowRuntimeActionPresenter
{
    private readonly IWorkflowRuntimeActionViewModelFactory _viewModelFactory;

    public WorkflowRuntimeActionPresenter(
        IWorkflowRuntimeActionViewModelFactory viewModelFactory)
    {
        ArgumentNullException.ThrowIfNull(viewModelFactory);

        _viewModelFactory = viewModelFactory;
    }

    public string Present(
        string actionName,
        WorkflowRuntimeActionResult result,
        DateTimeOffset completedUtc)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(actionName);
        ArgumentNullException.ThrowIfNull(result);

        WorkflowRuntimeActionViewModel viewModel =
            _viewModelFactory.Create(
                actionName,
                result,
                completedUtc);

        return WorkflowRuntimeActionTextFormatter.Format(viewModel);
    }
}
