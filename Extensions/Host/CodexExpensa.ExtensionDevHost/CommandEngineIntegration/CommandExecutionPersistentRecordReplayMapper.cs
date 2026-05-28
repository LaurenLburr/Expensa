namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration;

public static class CommandExecutionPersistentRecordReplayMapper
{
    public static string GetCommandName(
        CommandExecutionPersistentRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);

        return record.CommandName;
    }

    public static string GetParameterJson(
        CommandExecutionPersistentRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);

        return string.IsNullOrWhiteSpace(record.ParameterJson)
            ? "{}"
            : record.ParameterJson;
    }

    public static bool CanReplay(
        CommandExecutionPersistentRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);

        return !string.IsNullOrWhiteSpace(record.CommandName);
    }
}
