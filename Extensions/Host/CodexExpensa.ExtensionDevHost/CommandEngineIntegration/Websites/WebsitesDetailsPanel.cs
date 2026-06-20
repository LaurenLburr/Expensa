using System.Windows.Forms.Integration;
using System.Text;
using CodexExpensa.Core.Abstractions;
using CodexExpensa.Security.Windows;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed class WebsitesDetailsPanel : UserControl
{
    private readonly ICredentialStore credentialStore = new WindowsCredentialStore();
    private readonly ElementHost detailsHost = new();
    private readonly WebsiteDetailsWpfControl detailsControl = new();
    private readonly WebsiteOverviewWpfControl overviewControl = new();

    public event EventHandler<string>? WebsiteOverviewRowDoubleClicked;

    public WebsitesDetailsPanel()
    {
        detailsHost.Dock = DockStyle.Fill;
        detailsHost.Child = detailsControl;
        detailsControl.ApplyPasswordToWebRequested += detailsControl_ApplyPasswordToWebRequested;
        detailsControl.SaveCredentialRequested += detailsControl_SaveCredentialRequested;
        detailsControl.ImportCredentialRequested += detailsControl_ImportCredentialRequested;
        detailsControl.RefreshCredentialsRequested += detailsControl_RefreshCredentialsRequested;
        overviewControl.WebsiteRowDoubleClicked += overviewControl_WebsiteRowDoubleClicked;

        Controls.Add(detailsHost);
        Dock = DockStyle.Fill;

        ShowEmpty();
    }

    public void ShowWebsite(HostWebsiteTreeNodePayload payload)
    {
        ArgumentNullException.ThrowIfNull(payload);

        detailsHost.Child = detailsControl;

        string credentialKey = CreateWebsiteCredentialKey(payload.WebsiteId);
        credentialStore.TryGet(credentialKey, out string? username, out string? password);

        detailsControl.ShowWebsite(
            name: payload.DisplayText,
            credentialKey: credentialKey,
            username: username ?? string.Empty,
            password: password ?? string.Empty,
            websiteId: payload.WebsiteId,
            url: payload.Url,
            tagName: payload.TagName,
            isActive: payload.IsActive);
    }

    public void ShowGroup(IHostWebsiteTreeNodePayload payload)
    {
        ArgumentNullException.ThrowIfNull(payload);

        detailsHost.Child = detailsControl;

        detailsControl.ShowGroup(
            name: payload.DisplayText,
            nodeId: payload.NodeId,
            nodeType: payload.NodeType.ToString());
    }

    public void ShowOverview(IEnumerable<HostWebsiteTreeNodePayload> payloads)
    {
        detailsHost.Child = overviewControl;
        overviewControl.SetWebsites(payloads);
    }

    public void ShowEmpty()
    {
        detailsHost.Child = detailsControl;
        detailsControl.ShowEmpty();
    }

    public void SaveCurrentCredential()
    {
        if (!detailsControl.TryCreateCredentialRequest(
                out WebsiteCredentialApplyRequest? request,
                out string message))
        {
            if (message != "No website credential key is selected." &&
                message != "No credential values to save.")
            {
                detailsControl.ShowCredentialStatus(message);
            }

            return;
        }

        SaveCredential(request!);
    }

    private static string CreateWebsiteCredentialKey(string websiteId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(websiteId);

        return $"CodexExpensa.Website.{websiteId}";
    }

    private void detailsControl_ApplyPasswordToWebRequested(object? sender, WebsiteCredentialApplyRequest e)
    {
        SaveCredential(e);
    }

    private void detailsControl_SaveCredentialRequested(object? sender, WebsiteCredentialApplyRequest e)
    {
        SaveCredential(e);
    }

    private void SaveCredential(WebsiteCredentialApplyRequest e)
    {
        try
        {
            credentialStore.Save(e.CredentialKey, e.Username, e.Password);
            detailsControl.ShowCredentialStatus("Updated Windows Credential Manager.");
        }
        catch (Exception exception)
        {
            detailsControl.ShowCredentialStatus($"Credential update failed: {exception.Message}");
        }
    }

    private void detailsControl_RefreshCredentialsRequested(object? sender, string credentialKey)
    {
        try
        {
            credentialStore.TryGet(credentialKey, out string? username, out string? password);

            detailsControl.UpdateCredentialValues(
                username ?? string.Empty,
                password ?? string.Empty);
        }
        catch (Exception exception)
        {
            detailsControl.ShowCredentialStatus($"Credential refresh failed: {exception.Message}");
        }
    }

    private void overviewControl_WebsiteRowDoubleClicked(object? sender, string websiteId)
    {
        WebsiteOverviewRowDoubleClicked?.Invoke(this, websiteId);
    }

    private void detailsControl_ImportCredentialRequested(object? sender, WebsiteCredentialImportRequest e)
    {
        using OpenFileDialog dialog = new()
        {
            Title = "Import Website Credentials",
            Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
            CheckFileExists = true,
            Multiselect = false
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            WebsiteCredentialCsvRecord? record =
                FindCredentialRecord(dialog.FileName, e.Url);

            if (record is null)
            {
                detailsControl.ShowCredentialStatus("No matching credential row was found in the selected CSV.");
                return;
            }

            credentialStore.Save(e.CredentialKey, record.Username, record.Password);
            detailsControl.UpdateCredentialValues(record.Username, record.Password);
            detailsControl.ShowCredentialStatus("Imported credential into Windows Credential Manager.");
        }
        catch (Exception exception)
        {
            detailsControl.ShowCredentialStatus($"Credential import failed: {exception.Message}");
        }
    }

    private static WebsiteCredentialCsvRecord? FindCredentialRecord(string fileName, string websiteUrl)
    {
        using StreamReader reader = new(fileName);

        string? headerLine = reader.ReadLine();

        if (string.IsNullOrWhiteSpace(headerLine))
        {
            return null;
        }

        List<string> headers = ParseCsvLine(headerLine);
        int urlIndex = FindHeaderIndex(headers, "url");
        int usernameIndex = FindHeaderIndex(headers, "username");
        int passwordIndex = FindHeaderIndex(headers, "password");

        if (urlIndex < 0 || usernameIndex < 0 || passwordIndex < 0)
        {
            throw new InvalidOperationException("CSV must include url, username, and password columns.");
        }

        Uri? selectedUri = TryCreateUri(websiteUrl);

        while (!reader.EndOfStream)
        {
            string? line = reader.ReadLine();

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            List<string> fields = ParseCsvLine(line);

            if (fields.Count <= Math.Max(urlIndex, Math.Max(usernameIndex, passwordIndex)))
            {
                continue;
            }

            string rowUrl = fields[urlIndex];

            if (!IsMatchingUrl(selectedUri, websiteUrl, rowUrl))
            {
                continue;
            }

            return new WebsiteCredentialCsvRecord(
                fields[usernameIndex],
                fields[passwordIndex]);
        }

        return null;
    }

    private static int FindHeaderIndex(IReadOnlyList<string> headers, string name)
    {
        for (int i = 0; i < headers.Count; i++)
        {
            if (string.Equals(headers[i], name, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }

        return -1;
    }

    private static bool IsMatchingUrl(Uri? selectedUri, string selectedUrl, string rowUrl)
    {
        Uri? rowUri = TryCreateUri(rowUrl);

        if (selectedUri is not null && rowUri is not null)
        {
            return string.Equals(selectedUri.Host, rowUri.Host, StringComparison.OrdinalIgnoreCase);
        }

        return !string.IsNullOrWhiteSpace(selectedUrl) &&
            rowUrl.Contains(selectedUrl, StringComparison.OrdinalIgnoreCase);
    }

    private static Uri? TryCreateUri(string value)
    {
        return Uri.TryCreate(value, UriKind.Absolute, out Uri? uri)
            ? uri
            : null;
    }

    private static List<string> ParseCsvLine(string line)
    {
        List<string> fields = [];
        StringBuilder fieldBuilder = new();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char current = line[i];

            if (current == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    fieldBuilder.Append('"');
                    i++;
                    continue;
                }

                inQuotes = !inQuotes;
                continue;
            }

            if (current == ',' && !inQuotes)
            {
                fields.Add(fieldBuilder.ToString());
                fieldBuilder.Clear();
                continue;
            }

            fieldBuilder.Append(current);
        }

        fields.Add(fieldBuilder.ToString());

        return fields;
    }

    private sealed record WebsiteCredentialCsvRecord(
        string Username,
        string Password);
}
