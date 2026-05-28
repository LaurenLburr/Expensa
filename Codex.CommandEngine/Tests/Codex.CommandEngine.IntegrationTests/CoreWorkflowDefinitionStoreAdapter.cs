using CoreWorkflowDefinitionDocument = Codex.CommandEngine.Core.WorkflowDefinitionDocument;
using CoreWorkflowDefinitionStep = Codex.CommandEngine.Core.WorkflowDefinitionStep;
using CoreIWorkflowDefinitionStore = Codex.CommandEngine.Core.IWorkflowDefinitionStore;
using DataWorkflowDefinitionDocument = Codex.CommandEngine.Data.WorkflowDefinitionDocument;
using DataWorkflowDefinitionRepository = Codex.CommandEngine.Data.WorkflowDefinitionRepository;
using DataWorkflowDefinitionStep = Codex.CommandEngine.Data.WorkflowDefinitionStep;

namespace Codex.CommandEngine.IntegrationTests;

internal sealed class CoreWorkflowDefinitionStoreAdapter : CoreIWorkflowDefinitionStore
{
    private readonly DataWorkflowDefinitionRepository _repository;

    public CoreWorkflowDefinitionStoreAdapter(DataWorkflowDefinitionRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);

        _repository = repository;
    }

    public CoreWorkflowDefinitionDocument? FindByName(string workflowName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workflowName);

        DataWorkflowDefinitionDocument? document =
            _repository.FindByName(workflowName);

        return document is null
            ? null
            : ConvertDocument(document);
    }

    public IReadOnlyList<CoreWorkflowDefinitionDocument> ListActive()
    {
        return _repository
            .ListActive()
            .Select(ConvertDocument)
            .ToList();
    }

    private static CoreWorkflowDefinitionDocument ConvertDocument(
        DataWorkflowDefinitionDocument document)
    {
        return new CoreWorkflowDefinitionDocument
        {
            WorkflowDefinitionId = document.WorkflowDefinitionId,
            WorkflowName = document.WorkflowName,
            DisplayName = document.DisplayName,
            Description = document.Description,
            Version = document.Version,
            IsActive = document.IsActive,
            Steps = document.Steps
                .Select(ConvertStep)
                .ToList()
        };
    }

    private static CoreWorkflowDefinitionStep ConvertStep(
        DataWorkflowDefinitionStep step)
    {
        return new CoreWorkflowDefinitionStep
        {
            StepName = step.StepName,
            StepOrder = step.StepOrder,
            CommandName = step.CommandName,
            Parameters = step.Parameters
        };
    }
}
