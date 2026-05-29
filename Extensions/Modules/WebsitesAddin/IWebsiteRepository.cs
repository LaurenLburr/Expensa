namespace WebsitesAddin;

public interface IWebsiteRepository
{
    IReadOnlyList<WebsiteTreeNode> LoadWebsites(
        WebsiteLoadRequest request);
}
