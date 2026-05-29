namespace WebsitesAddin;

public sealed class WebsitesTreeContributionProvider
{
    public string AddinId => "websites";

    public string DisplayName => "Websites";

    public int SortOrder => 100;

    public string CommandName => "websites.load";

    public string RootNodeName => "websites";

    public string RootDisplayText => "Websites";
}
