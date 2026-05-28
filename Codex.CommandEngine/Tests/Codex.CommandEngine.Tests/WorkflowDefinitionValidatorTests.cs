using Codex.CommandEngine.Core;
using Xunit;

namespace Codex.CommandEngine.Tests;

public sealed class WorkflowDefinitionValidatorTests
{
    [Fact]
    public void Validate_WhenDefinitionIsValid_ReturnsValidResult()
    {
        WorkflowDefinitionValidator validator = new();

        WorkflowDefinitionValidationResult result =
            validator.Validate(CreateValidDefinition());

        Assert.True(result.IsValid);
        Assert.Empty(result.Issues);
    }

    [Fact]
    public void Validate_WhenWorkflowNameMissing_ReturnsIssue()
    {
        WorkflowDefinitionValidator validator = new();

        WorkflowDefinitionDocument definition =
            new()
            {
                WorkflowDefinitionId = "workflow-definition-001",
                WorkflowName = "",
                Version = 1,
                Steps =
                [
                    new WorkflowDefinitionStep
                    {
                        StepName = "First",
                        StepOrder = 1,
                        CommandName = "first.command"
                    }
                ]
            };

        WorkflowDefinitionValidationResult result =
            validator.Validate(definition);

        Assert.False(result.IsValid);
        Assert.Contains(result.Issues, issue => issue.Code == "WorkflowName.Required");
    }

    [Fact]
    public void Validate_WhenNoSteps_ReturnsIssue()
    {
        WorkflowDefinitionValidator validator = new();

        WorkflowDefinitionDocument definition =
            new()
            {
                WorkflowDefinitionId = "workflow-definition-001",
                WorkflowName = "empty.workflow",
                Version = 1,
                Steps = []
            };

        WorkflowDefinitionValidationResult result =
            validator.Validate(definition);

        Assert.False(result.IsValid);
        Assert.Contains(result.Issues, issue => issue.Code == "Steps.Required");
    }

    [Fact]
    public void Validate_WhenStepOrderDuplicated_ReturnsIssue()
    {
        WorkflowDefinitionValidator validator = new();

        WorkflowDefinitionDocument definition =
            new()
            {
                WorkflowDefinitionId = "workflow-definition-001",
                WorkflowName = "duplicate.workflow",
                Version = 1,
                Steps =
                [
                    new WorkflowDefinitionStep
                    {
                        StepName = "First",
                        StepOrder = 1,
                        CommandName = "first.command"
                    },
                    new WorkflowDefinitionStep
                    {
                        StepName = "Second",
                        StepOrder = 1,
                        CommandName = "second.command"
                    }
                ]
            };

        WorkflowDefinitionValidationResult result =
            validator.Validate(definition);

        Assert.False(result.IsValid);
        Assert.Contains(result.Issues, issue => issue.Code == "StepOrder.Duplicate");
    }

    [Fact]
    public void Format_WhenInvalid_ReturnsReadableValidationText()
    {
        WorkflowDefinitionDocument definition =
            new()
            {
                WorkflowDefinitionId = "workflow-definition-001",
                WorkflowName = "bad.workflow",
                Version = 0,
                Steps = []
            };

        WorkflowDefinitionValidator validator = new();

        string text =
            WorkflowDefinitionValidationTextFormatter.Format(
                definition,
                validator.Validate(definition));

        Assert.Contains("Workflow Definition Validation", text);
        Assert.Contains("Is Valid: False", text);
        Assert.Contains("Version.Invalid", text);
        Assert.Contains("Steps.Required", text);
    }

    private static WorkflowDefinitionDocument CreateValidDefinition()
    {
        return new WorkflowDefinitionDocument
        {
            WorkflowDefinitionId = "workflow-definition-001",
            WorkflowName = "valid.workflow",
            Version = 1,
            Steps =
            [
                new WorkflowDefinitionStep
                {
                    StepName = "First",
                    StepOrder = 1,
                    CommandName = "first.command"
                },
                new WorkflowDefinitionStep
                {
                    StepName = "Second",
                    StepOrder = 2,
                    CommandName = "second.command"
                }
            ]
        };
    }
}
