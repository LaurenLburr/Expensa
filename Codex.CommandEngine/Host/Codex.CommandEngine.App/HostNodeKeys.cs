namespace Codex.CommandEngine.App;

public static class HostNodeKeys
{
    public const string DatabaseSchema = "database.schema";
    public const string DatabaseFullSchema = "database.fullSchema";
    public const string SqlCatalog = "database.sqlCatalog";

    public const string CommandDefinitions = "definitions.commands";
    public const string WorkflowDefinitions = "definitions.workflows";
    public const string ContextDefinitions = "definitions.contexts";
    public const string AiProviderDefinitions = "definitions.aiProviders";

    public const string ExecutionHistory = "execution.history";

    public const string RuntimeOperations = "runtime.operations";
    public const string RuntimeResumableWorkflows = "runtime.resumableWorkflows";
    public const string RuntimeStaleWorkflows = "runtime.staleWorkflows";
    public const string RuntimeIncompleteWorkflows = "runtime.incompleteWorkflows";
}
