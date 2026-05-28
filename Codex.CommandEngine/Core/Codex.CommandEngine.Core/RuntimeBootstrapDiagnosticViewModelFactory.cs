namespace Codex.CommandEngine.Core;

public static class RuntimeBootstrapDiagnosticViewModelFactory
{
    public static RuntimeBootstrapDiagnosticViewModel Create(
        CommandEngineRuntimeBootstrapResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return Create(
            result.RegisteredCommands.Count,
            result.Diagnostics);
    }

    public static RuntimeBootstrapDiagnosticViewModel Create(
        ExtensionCommandRuntimeBootstrapResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return Create(
            result.RegisteredCommands.Count,
            result.Diagnostics);
    }

    private static RuntimeBootstrapDiagnosticViewModel Create(
        int registeredCommandCount,
        RuntimeBootstrapDiagnostics diagnostics)
    {
        ArgumentNullException.ThrowIfNull(diagnostics);

        IReadOnlyList<RuntimeBootstrapDiagnosticLine> lines =
            diagnostics.Issues
                .Select(static issue => new RuntimeBootstrapDiagnosticLine
                {
                    Severity = issue.Severity,
                    CommandName = issue.CommandName,
                    Message = issue.Message
                })
                .ToList();

        return new RuntimeBootstrapDiagnosticViewModel
        {
            RegisteredCommandCount = registeredCommandCount,
            IssueCount = lines.Count,
            HasErrors = diagnostics.HasErrors,
            HasWarnings = diagnostics.HasWarnings,
            Lines = lines
        };
    }
}
