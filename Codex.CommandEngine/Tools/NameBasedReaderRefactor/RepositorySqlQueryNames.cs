namespace Codex.CommandEngine.Data;

public static class RepositorySqlQueryNames
{
    public static class SqlQuery
    {
        public const string SelectByName = "SqlQuery_SelectByName";
        public const string SelectAll = "SqlQuery_SelectAll";
        public const string InsertOrReplace = "SqlQuery_InsertOrReplace";
    }

    public static class AiProvider
    {
        public const string InsertOrReplace = "AiProvider_InsertOrReplace";
        public const string SelectByName = "AiProvider_SelectByName";
        public const string SelectAll = "AiProvider_SelectAll";
    }

    public static class AiProviderCapability
    {
        public const string InsertOrReplace = "AiProviderCapability_InsertOrReplace";
        public const string SelectByProvider = "AiProviderCapability_SelectByProvider";
    }

    public static class CommandDefinition
    {
        public const string InsertOrReplace = "CommandDefinition_InsertOrReplace";
        public const string SelectByName = "CommandDefinition_SelectByName";
        public const string SelectAll = "CommandDefinition_SelectAll";
    }

    public static class CommandParameterDefinition
    {
        public const string InsertOrReplace = "CommandParameterDefinition_InsertOrReplace";
        public const string SelectByCommand = "CommandParameterDefinition_SelectByCommand";
    }

    public static class EngineContext
    {
        public const string InsertOrReplace = "EngineContext_InsertOrReplace";
        public const string SelectByName = "EngineContext_SelectByName";
        public const string SelectAll = "EngineContext_SelectAll";
    }

    public static class WorkflowDefinition
    {
        public const string InsertOrReplace = "WorkflowDefinition_InsertOrReplace";
        public const string SelectByNameAndVersion = "WorkflowDefinition_SelectByNameAndVersion";
        public const string SelectAll = "WorkflowDefinition_SelectAll";
    }

    public static class WorkflowStep
    {
        public const string InsertOrReplace = "WorkflowStep_InsertOrReplace";
        public const string SelectByWorkflow = "WorkflowStep_SelectByWorkflow";
    }

    public static class ExecutionHistory
    {
        public const string Insert = "ExecutionHistory_Insert";
        public const string Complete = "ExecutionHistory_Complete";
        public const string SelectById = "ExecutionHistory_SelectById";
        public const string SelectRecent = "ExecutionHistory_SelectRecent";
    }

    public static class ExecutionStepHistory
    {
        public const string Insert = "ExecutionStepHistory_Insert";
        public const string Complete = "ExecutionStepHistory_Complete";
        public const string SelectByExecution = "ExecutionStepHistory_SelectByExecution";
    }

    public static IReadOnlyList<string> AllRequiredRepositoryQueries { get; } =
    [
        SqlQuery.SelectByName,
        SqlQuery.SelectAll,
        SqlQuery.InsertOrReplace,

        AiProvider.InsertOrReplace,
        AiProvider.SelectByName,
        AiProvider.SelectAll,
        AiProviderCapability.InsertOrReplace,
        AiProviderCapability.SelectByProvider,

        CommandDefinition.InsertOrReplace,
        CommandDefinition.SelectByName,
        CommandDefinition.SelectAll,
        CommandParameterDefinition.InsertOrReplace,
        CommandParameterDefinition.SelectByCommand,

        EngineContext.InsertOrReplace,
        EngineContext.SelectByName,
        EngineContext.SelectAll,

        WorkflowDefinition.InsertOrReplace,
        WorkflowDefinition.SelectByNameAndVersion,
        WorkflowDefinition.SelectAll,
        WorkflowStep.InsertOrReplace,
        WorkflowStep.SelectByWorkflow,

        ExecutionHistory.Insert,
        ExecutionHistory.Complete,
        ExecutionHistory.SelectById,
        ExecutionHistory.SelectRecent,
        ExecutionStepHistory.Insert,
        ExecutionStepHistory.Complete,
        ExecutionStepHistory.SelectByExecution
    ];
}
