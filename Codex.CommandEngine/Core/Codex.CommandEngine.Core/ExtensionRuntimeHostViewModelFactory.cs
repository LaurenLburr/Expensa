namespace Codex.CommandEngine.Core;

public static class ExtensionRuntimeHostViewModelFactory
{
    public static ExtensionRuntimeHostViewModel Create(
        ExtensionRuntimeHostSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        IReadOnlyList<ExtensionRuntimeHostCommandViewModel> commands =
            snapshot.Commands
                .OrderBy(static command => command.Category, StringComparer.OrdinalIgnoreCase)
                .ThenBy(static command => command.CommandName, StringComparer.OrdinalIgnoreCase)
                .Select(static command => new ExtensionRuntimeHostCommandViewModel
                {
                    CommandName = command.CommandName,
                    DisplayName = command.DisplayName,
                    Category = command.Category,
                    Description = command.Description,
                    Version = command.Version,
                    IsEnabled = command.IsEnabled
                })
                .ToList();

        IReadOnlyList<ExtensionRuntimeHostDiagnosticViewModel> diagnostics =
            snapshot.Diagnostics.Issues
                .Select(static issue => new ExtensionRuntimeHostDiagnosticViewModel
                {
                    Severity = issue.Severity,
                    CommandName = issue.CommandName,
                    Message = issue.Message
                })
                .ToList();

        return new ExtensionRuntimeHostViewModel
        {
            State = snapshot.State,
            StateText = snapshot.State.ToString(),
            RegisteredCommandCount = commands.Count,
            DiagnosticCount = diagnostics.Count,
            HasErrors = snapshot.Diagnostics.HasErrors,
            HasWarnings = snapshot.Diagnostics.HasWarnings,
            Summary = BuildSummary(snapshot.State, commands.Count, diagnostics.Count, snapshot.Diagnostics.HasErrors),
            Commands = commands,
            Diagnostics = diagnostics
        };
    }

    private static string BuildSummary(
        ExtensionRuntimeHostState state,
        int commandCount,
        int diagnosticCount,
        bool hasErrors)
    {
        return state switch
        {
            ExtensionRuntimeHostState.NotStarted =>
                "Runtime has not been started.",

            ExtensionRuntimeHostState.Stopped =>
                "Runtime is stopped.",

            ExtensionRuntimeHostState.Failed when hasErrors =>
                $"Runtime failed with {diagnosticCount} diagnostic issue(s).",

            ExtensionRuntimeHostState.Failed =>
                "Runtime failed.",

            ExtensionRuntimeHostState.Started =>
                $"Runtime started with {commandCount} command(s).",

            _ =>
                $"Runtime state is {state}."
        };
    }
}
