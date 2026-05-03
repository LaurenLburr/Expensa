namespace CodexExpensa.ExtensionDevHost.Models;

public sealed class ExtensionProjectRegistration
{
    public string ProjectName { get; set; } = string.Empty;
    public string? RelativeBinPath { get; set; }
    public string AssemblyName { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public int SortOrder { get; set; }
}
