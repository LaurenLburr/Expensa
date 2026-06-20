namespace CodexExpensa.ExtensionDevHost.Deployment;

public sealed class DeploymentPathPolicy
{
    private readonly string _repositoryRoot;

    public DeploymentPathPolicy(string repositoryRoot)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(repositoryRoot);

        _repositoryRoot =
            NormalizeRoot(repositoryRoot);
    }

    public string ResolveRepositoryPath(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string candidate =
            Path.IsPathRooted(path)
                ? Path.GetFullPath(path)
                : Path.GetFullPath(
                    Path.Combine(
                        _repositoryRoot,
                        path));

        EnsureWithinRepository(candidate);

        return candidate;
    }

    public string ResolveTargetPath(
        string targetRoot,
        string path)
    {
        string resolvedTargetRoot =
            ResolveRepositoryPath(targetRoot);

        string candidate =
            Path.IsPathRooted(path)
                ? Path.GetFullPath(path)
                : Path.GetFullPath(
                    Path.Combine(
                        resolvedTargetRoot,
                        path));

        EnsureWithinRepository(candidate);

        return candidate;
    }

    private void EnsureWithinRepository(string candidate)
    {
        string normalized =
            Path.GetFullPath(candidate);

        string repositoryRootWithoutSeparator =
            _repositoryRoot.TrimEnd(
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar);

        bool isRepositoryRoot =
            string.Equals(
                normalized.TrimEnd(
                    Path.DirectorySeparatorChar,
                    Path.AltDirectorySeparatorChar),
                repositoryRootWithoutSeparator,
                StringComparison.OrdinalIgnoreCase);

        bool isBelowRepositoryRoot =
            normalized.StartsWith(
                _repositoryRoot,
                StringComparison.OrdinalIgnoreCase);

        if (!isRepositoryRoot &&
            !isBelowRepositoryRoot)
        {
            throw new InvalidOperationException(
                $"Deployment path is outside the repository root:{Environment.NewLine}{normalized}");
        }
    }

    private static string NormalizeRoot(string path)
    {
        string full =
            Path.GetFullPath(path)
                .TrimEnd(
                    Path.DirectorySeparatorChar,
                    Path.AltDirectorySeparatorChar);

        return full + Path.DirectorySeparatorChar;
    }
}
