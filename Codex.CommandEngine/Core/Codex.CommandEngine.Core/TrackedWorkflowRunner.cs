using System.Diagnostics;

namespace Codex.CommandEngine.Core;

public sealed class TrackedWorkflowRunner : IWorkflowRunner
{
    private readonly IWorkflowRunner _innerRunner;
    private readonly IWorkflowExecutionHistorySink _historySink;

    public TrackedWorkflowRunner(
        IWorkflowRunner innerRunner,
        IWorkflowExecutionHistorySink historySink)
    {
        ArgumentNullException.ThrowIfNull(innerRunner);
        ArgumentNullException.ThrowIfNull(historySink);

        _innerRunner = innerRunner;
        _historySink = historySink;
    }

    public async Task<WorkflowExecutionResult> ExecuteAsync(
        WorkflowExecutionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.WorkflowName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.CorrelationId);

        string workflowExecutionId = Guid.NewGuid().ToString("N");
        DateTimeOffset startedUtc = DateTimeOffset.UtcNow;
        Stopwatch stopwatch = Stopwatch.StartNew();

        _historySink.Started(request, workflowExecutionId, startedUtc);

        WorkflowExecutionResult result;

        try
        {
            result = await _innerRunner
                .ExecuteAsync(request, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            result = new WorkflowExecutionResult
            {
                WorkflowName = request.WorkflowName,
                CorrelationId = request.CorrelationId,
                Status = WorkflowExecutionStatus.Cancelled,
                Message = "Workflow execution was cancelled."
            };
        }
        catch (Exception exception)
        {
            result = new WorkflowExecutionResult
            {
                WorkflowName = request.WorkflowName,
                CorrelationId = request.CorrelationId,
                Status = WorkflowExecutionStatus.Failed,
                Message = exception.Message
            };
        }

        stopwatch.Stop();

        _historySink.Completed(
            request,
            result,
            workflowExecutionId,
            DateTimeOffset.UtcNow,
            stopwatch.ElapsedMilliseconds);

        return result;
    }
}
