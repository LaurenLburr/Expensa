namespace WebsitesAddin;

public sealed class WebsiteLoadCommand
{
    private readonly IWebsiteRepository _repository;

    public WebsiteLoadCommand()
        : this(new InMemoryWebsiteRepository())
    {
    }

    public WebsiteLoadCommand(
        IWebsiteRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);

        _repository = repository;
    }

    public string Name =>
        "websites.load";

    public WebsiteLoadResult Execute(
        WebsiteLoadRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        IReadOnlyList<WebsiteTreeNode> nodes =
            _repository.LoadWebsites(request);

        return new WebsiteLoadResult
        {
            Nodes = nodes,
            Message = $"Loaded {nodes.Count} website node(s)."
        };
    }

    public WebsiteLoadResult Execute(
        IReadOnlyDictionary<string, object?> parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        return Execute(
            WebsiteLoadRequestParser.Parse(parameters));
    }
}
