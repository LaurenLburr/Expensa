namespace WebsitesAddin;

public sealed class InMemoryWebsiteRepository : IWebsiteRepository
{
    private readonly IReadOnlyList<WebsiteTreeNode> _nodes;

    public InMemoryWebsiteRepository()
        : this(
            [
                new WebsiteTreeNode
                {
                    NodeId = "banking",
                    DisplayText = "Banking",
                    Category = "Root",
                    Children =
                    [
                        new WebsiteTreeNode
                        {
                            NodeId = "banking.demo",
                            DisplayText = "Demo Bank",
                            Url = "https://example.com/bank",
                            Category = "Banking"
                        }
                    ]
                },
                new WebsiteTreeNode
                {
                    NodeId = "utilities",
                    DisplayText = "Utilities",
                    Category = "Root",
                    Children =
                    [
                        new WebsiteTreeNode
                        {
                            NodeId = "utilities.demo",
                            DisplayText = "Demo Utility",
                            Url = "https://example.com/utility",
                            Category = "Utilities"
                        }
                    ]
                }
            ])
    {
    }

    public InMemoryWebsiteRepository(
        IReadOnlyList<WebsiteTreeNode> nodes)
    {
        ArgumentNullException.ThrowIfNull(nodes);

        _nodes = nodes;
    }

    public IReadOnlyList<WebsiteTreeNode> LoadWebsites(
        WebsiteLoadRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        int maximumRows =
            Math.Max(1, request.MaximumRows);

        IEnumerable<WebsiteTreeNode> query =
            _nodes;

        if (!request.IncludeDisabled)
        {
            query =
                query.Where(static node => node.IsEnabled);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            string search =
                request.SearchText.Trim();

            query =
                query.Where(node => Matches(node, search));
        }

        return query
            .Take(maximumRows)
            .ToList();
    }

    private static bool Matches(
        WebsiteTreeNode node,
        string search)
    {
        if (node.DisplayText.Contains(search, StringComparison.OrdinalIgnoreCase) ||
            node.Url.Contains(search, StringComparison.OrdinalIgnoreCase) ||
            node.Category.Contains(search, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return node.Children.Any(child => Matches(child, search));
    }
}
