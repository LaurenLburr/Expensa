using CodexExpensa.Navigation.Abstractions;

namespace WebsitesAddin;

/// <summary>
/// Blank Expensa add-in scaffold.
/// Smart scaffold prompt captured in README.md / ScaffoldPrompt.md.
/// </summary>
public sealed class WebsitesAddinExtension : ITreeNodeExtension
{
    public string ExtensionKey => "WebsitesAddin";

    public int SortOrder => 100;
}
