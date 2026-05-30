namespace WebsitesAddin;

public sealed class WebsiteLoadCommand
{
    private readonly IWebsiteRepository _repository;

    public WebsiteLoadCommand()
        : this(CreateDefaultRepository())
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
            Message = $"Loaded {nodes.Count} website root node(s)."
        };
    }

    public WebsiteLoadResult Execute(
        IReadOnlyDictionary<string, object?> parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        return Execute(
            WebsiteLoadRequestParser.Parse(parameters));
    }

    private static IWebsiteRepository CreateDefaultRepository()
    {
        string devDatabasePath =
            WebsiteDatabasePathResolver.ResolveDevDatabasePath();

        if (File.Exists(devDatabasePath))
        {
            return new SqliteWebsiteRepository(
                new WebsiteDatabaseOptions
                {
                    DatabasePath = devDatabasePath
                });
        }

        return new InMemoryWebsiteRepository();
    }
}
