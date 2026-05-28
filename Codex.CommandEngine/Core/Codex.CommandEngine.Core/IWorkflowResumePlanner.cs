namespace Codex.CommandEngine.Core;

public interface IWorkflowResumePlanner
{
    WorkflowResumePlan CreateResumePlan(WorkflowResumeRequest request);
}
