namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

public sealed class AddinDevDatabaseUiCoordinator
{
    private readonly AddinRuntimeDatabasePathService _pathService;
    private readonly AddinDevDatabaseCopyService _copyService;

    public AddinDevDatabaseUiCoordinator()
        : this(
            new AddinRuntimeDatabasePathService(),
            new AddinDevDatabaseCopyService())
    {
    }

    public AddinDevDatabaseUiCoordinator(
        AddinRuntimeDatabasePathService pathService,
        AddinDevDatabaseCopyService copyService)
    {
        ArgumentNullException.ThrowIfNull(pathService);
        ArgumentNullException.ThrowIfNull(copyService);
        _pathService = pathService;
        _copyService = copyService;
    }

    public bool EnsureDevDatabaseExists(IWin32Window owner, string addinId)
    {
        string devPath = _pathService.GetDevCurrentDatabasePath(addinId);

        if (File.Exists(devPath))
        {
            return true;
        }

        DialogResult answer = MessageBox.Show(
            owner,
            $"The {addinId} Dev database has not been created.{Environment.NewLine}{Environment.NewLine}" +
            $"Expected file:{Environment.NewLine}{devPath}{Environment.NewLine}{Environment.NewLine}" +
            "Create it now by copying the current add-in database?",
            "Create Dev Database",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question,
            MessageBoxDefaultButton.Button1);

        if (answer != DialogResult.Yes)
        {
            return false;
        }

        try
        {
            AddinDevDatabaseCopyResult result =
                _copyService.CopyRuntimeDatabaseToDev(addinId);

            ShowSuccess(owner, result);
            return true;
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                owner,
                exception.Message,
                "Create Dev Database",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            return false;
        }
    }

    public AddinDevDatabaseCopyResult? CreateOrReplaceDevDatabase(
        IWin32Window owner,
        string addinId)
    {
        string devPath = _pathService.GetDevCurrentDatabasePath(addinId);

        if (File.Exists(devPath))
        {
            DialogResult answer = MessageBox.Show(
                owner,
                $"Replace the existing {addinId} Dev database with the current add-in database?{Environment.NewLine}{Environment.NewLine}" +
                $"Existing Dev file:{Environment.NewLine}{devPath}{Environment.NewLine}{Environment.NewLine}" +
                "The existing Dev file will be archived first.",
                "Replace Dev Database",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (answer != DialogResult.Yes)
            {
                return null;
            }
        }

        AddinDevDatabaseCopyResult result =
            _copyService.CopyRuntimeDatabaseToDev(addinId);

        ShowSuccess(owner, result);
        return result;
    }

    private static void ShowSuccess(
        IWin32Window owner,
        AddinDevDatabaseCopyResult result)
    {
        string archiveText = string.IsNullOrWhiteSpace(result.ReplacedDevDatabaseArchivePath)
            ? string.Empty
            : $"{Environment.NewLine}{Environment.NewLine}Archived previous Dev database:{Environment.NewLine}{result.ReplacedDevDatabaseArchivePath}";

        MessageBox.Show(
            owner,
            $"Dev database created from the current add-in database.{Environment.NewLine}{Environment.NewLine}" +
            $"Source:{Environment.NewLine}{result.RuntimeDatabasePath}{Environment.NewLine}{Environment.NewLine}" +
            $"Destination:{Environment.NewLine}{result.DevDatabasePath}" +
            archiveText,
            "Dev Database",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }
}
