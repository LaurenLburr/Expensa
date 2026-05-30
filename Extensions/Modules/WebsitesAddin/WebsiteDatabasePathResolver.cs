namespace WebsitesAddin;

public static class WebsiteDatabasePathResolver
{
    public static string ResolveDevDatabasePath()
    {
        return Path.Combine(
            FindModuleRoot(),
            "DevDatabase",
            "websites.dev.db");
    }

    private static string FindModuleRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            string candidate =
                Path.Combine(directory.FullName, "DevDatabase");

            if (Directory.Exists(candidate))
            {
                return directory.FullName;
            }

            DirectoryInfo? modulesParent =
                directory.Parent;

            if (modulesParent is not null)
            {
                string moduleCandidate =
                    Path.Combine(
                        modulesParent.FullName,
                        "Modules",
                        "WebsitesAddin");

                if (Directory.Exists(Path.Combine(moduleCandidate, "DevDatabase")))
                {
                    return moduleCandidate;
                }
            }

            directory = directory.Parent;
        }

        return Path.Combine(
            Directory.GetCurrentDirectory(),
            "DevDatabase");
    }
}
