using System.Windows;
using System.Security.Cryptography;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed partial class WebsiteDetailsWpfControl : System.Windows.Controls.UserControl
{
    private const string LowercaseCharacters = "abcdefghijklmnopqrstuvwxyz";
    private const string UppercaseCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string DigitCharacters = "0123456789";

    private bool _isSyncingPassword;

    public event EventHandler<WebsiteCredentialApplyRequest>? ApplyPasswordToWebRequested;
    public event EventHandler<WebsiteCredentialApplyRequest>? SaveCredentialRequested;
    public event EventHandler<WebsiteCredentialImportRequest>? ImportCredentialRequested;
    public event EventHandler<string>? RefreshCredentialsRequested;

    public WebsiteDetailsWpfControl()
    {
        InitializeComponent();
        SetEditable(false);
    }

    public void ShowWebsite(
        string name,
        string credentialKey,
        string username,
        string password,
        string websiteId,
        string url,
        string tagName,
        bool isActive)
    {
        EditableCheckBox.IsChecked = false;

        NameTextBox.Text = name;
        CredentialKeyTextBox.Text = credentialKey;
        UrlTextBox.Text = url;
        UsernameTextBox.Text = username;
        SetPassword(password);
        GeneratedPasswordLabel.Content = string.Empty;
        DetailsTextBox.Text =
            $"Name: {name}{Environment.NewLine}" +
            $"WebsiteId: {websiteId}{Environment.NewLine}" +
            $"CredentialKey: {credentialKey}{Environment.NewLine}" +
            $"Url: {url}{Environment.NewLine}" +
            $"Tag: {tagName}{Environment.NewLine}" +
            $"Active: {isActive}{Environment.NewLine}" +
            "Username and password are loaded from Windows Credential Manager by credential key.";

        SetEditable(false);
    }

    public void ShowGroup(
        string name,
        string nodeId,
        string nodeType)
    {
        EditableCheckBox.IsChecked = false;

        NameTextBox.Text = name;
        CredentialKeyTextBox.Text = string.Empty;
        UrlTextBox.Text = string.Empty;
        UsernameTextBox.Text = string.Empty;
        SetPassword(string.Empty);
        GeneratedPasswordLabel.Content = string.Empty;
        DetailsTextBox.Text =
            $"Group: {name}{Environment.NewLine}" +
            $"NodeId: {nodeId}{Environment.NewLine}" +
            $"NodeType: {nodeType}";

        SetEditable(false);
    }

    public void ShowEmpty()
    {
        EditableCheckBox.IsChecked = false;

        NameTextBox.Text = string.Empty;
        CredentialKeyTextBox.Text = string.Empty;
        UrlTextBox.Text = string.Empty;
        UsernameTextBox.Text = string.Empty;
        SetPassword(string.Empty);
        GeneratedPasswordLabel.Content = string.Empty;
        DetailsTextBox.Text = "Select a website node to load details.";

        SetEditable(false);
    }

    private void EditableCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
    {
        SetEditable(EditableCheckBox.IsChecked == true);
    }

    private void SetEditable(bool isEditable)
    {
        NameTextBox.IsReadOnly = !isEditable;
        CredentialKeyTextBox.IsReadOnly = true;
        UrlTextBox.IsReadOnly = true;
        UsernameTextBox.IsReadOnly = !isEditable;
        PasswordTextBox.IsReadOnly = !isEditable;
        PasswordMaskedBox.IsEnabled = false;
        PasswordMaskedBox.Visibility = isEditable
            ? Visibility.Collapsed
            : Visibility.Visible;
        PasswordTextBox.Visibility = isEditable
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    public void ShowCredentialStatus(string message)
    {
        DetailsTextBox.Text =
            DetailsTextBox.Text + Environment.NewLine + message;
    }

    private void GeneratePasswordButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string password = GeneratePassword(
                lowercaseCount: ReadNonNegativeCount(LowercaseCountTextBox.Text, "lowercase"),
                uppercaseCount: ReadNonNegativeCount(UppercaseCountTextBox.Text, "uppercase"),
                digitCount: ReadNonNegativeCount(DigitCountTextBox.Text, "digits"),
                specialCharacters: SpecialCharactersTextBox.Text,
                specialCount: ReadNonNegativeCount(SpecialCountTextBox.Text, "special"),
                minimumLength: ReadNonNegativeCount(MinimumLengthTextBox.Text, "minimum length"),
                maximumLength: ReadNonNegativeCount(MaximumLengthTextBox.Text, "maximum length"));

            GeneratedPasswordLabel.Content = password;
            SetPassword(password);
            ShowCredentialStatus("Generated password.");
        }
        catch (Exception exception)
        {
            ShowCredentialStatus($"Password generation failed: {exception.Message}");
        }
    }

    private void ApplyToWebHyperlink_Click(object sender, RoutedEventArgs e)
    {
        if (!TryCreateCredentialRequest(out WebsiteCredentialApplyRequest? request, out string message))
        {
            ShowCredentialStatus(message);
            return;
        }

        ApplyPasswordToWebRequested?.Invoke(
            this,
            request!);
    }

    private void SaveHyperlink_Click(object sender, RoutedEventArgs e)
    {
        if (!TryCreateCredentialRequest(out WebsiteCredentialApplyRequest? request, out string message))
        {
            ShowCredentialStatus(message);
            return;
        }

        SaveCredentialRequested?.Invoke(this, request!);
    }

    private void ImportHyperlink_Click(object sender, RoutedEventArgs e)
    {
        string credentialKey = CredentialKeyTextBox.Text.Trim();
        string url = UrlTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(credentialKey))
        {
            ShowCredentialStatus("No website credential key is selected.");
            return;
        }

        ImportCredentialRequested?.Invoke(
            this,
            new WebsiteCredentialImportRequest(
                credentialKey,
                url));
    }

    public bool TryCreateCredentialRequest(
        out WebsiteCredentialApplyRequest? request,
        out string message)
    {
        string credentialKey = CredentialKeyTextBox.Text.Trim();
        string username = UsernameTextBox.Text.Trim();
        string password = GetCurrentPassword();

        request = null;

        if (string.IsNullOrWhiteSpace(credentialKey))
        {
            message = "No website credential key is selected.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(username) && string.IsNullOrEmpty(password))
        {
            message = "No credential values to save.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(username))
        {
            message = "Enter a username before saving the password.";
            return false;
        }

        if (string.IsNullOrEmpty(password))
        {
            message = "Generate or enter a password before saving it.";
            return false;
        }

        SetPassword(password);

        request = new WebsiteCredentialApplyRequest(
            credentialKey,
            username,
            password);

        message = string.Empty;
        return true;
    }

    public void UpdateCredentialValues(string username, string password)
    {
        UsernameTextBox.Text = username;
        SetPassword(password);
        GeneratedPasswordLabel.Content = string.Empty;
        ShowCredentialStatus("Refreshed stored credentials.");
    }

    private void RefreshCredentialsButton_Click(object sender, RoutedEventArgs e)
    {
        string credentialKey = CredentialKeyTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(credentialKey))
        {
            ShowCredentialStatus("No website credential key is selected.");
            return;
        }

        RefreshCredentialsRequested?.Invoke(this, credentialKey);
    }

    private void CopyUrlHyperlink_Click(object sender, RoutedEventArgs e)
    {
        string url = UrlTextBox.Text.Trim();

        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
        {
            ShowCredentialStatus("Url is empty or invalid.");
            return;
        }

        try
        {
            System.Diagnostics.Process.Start(
                new System.Diagnostics.ProcessStartInfo
                {
                    FileName = uri.ToString(),
                    UseShellExecute = true
                });

            ShowCredentialStatus("Opened website.");
        }
        catch (Exception exception)
        {
            ShowCredentialStatus($"Open website failed: {exception.Message}");
        }
    }

    private void CopyUsernameHyperlink_Click(object sender, RoutedEventArgs e)
    {
        CopyValueToClipboard("Username", UsernameTextBox.Text);
    }

    private void CopyPasswordHyperlink_Click(object sender, RoutedEventArgs e)
    {
        CopyValueToClipboard("Password", GetCurrentPassword());
    }

    private void PasswordMaskedBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (_isSyncingPassword)
        {
            return;
        }

        _isSyncingPassword = true;
        PasswordTextBox.Text = PasswordMaskedBox.Password;
        _isSyncingPassword = false;
    }

    private void PasswordTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        if (_isSyncingPassword)
        {
            return;
        }

        _isSyncingPassword = true;
        PasswordMaskedBox.Password = PasswordTextBox.Text;
        _isSyncingPassword = false;
    }

    private void SetPassword(string password)
    {
        _isSyncingPassword = true;
        PasswordTextBox.Text = password;
        PasswordMaskedBox.Password = password;
        _isSyncingPassword = false;
    }

    private string GetCurrentPassword()
    {
        return PasswordTextBox.Visibility == Visibility.Visible
            ? PasswordTextBox.Text
            : PasswordMaskedBox.Password;
    }

    private void CopyValueToClipboard(string label, string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            ShowCredentialStatus($"{label} is empty.");
            return;
        }

        System.Windows.Clipboard.SetText(value);
        ShowCredentialStatus($"Copied {label} to clipboard.");
    }

    private static string GeneratePassword(
        int lowercaseCount,
        int uppercaseCount,
        int digitCount,
        string specialCharacters,
        int specialCount,
        int minimumLength,
        int maximumLength)
    {
        if (specialCount > 0 && string.IsNullOrEmpty(specialCharacters))
        {
            throw new InvalidOperationException("Special characters are required when special count is greater than zero.");
        }

        if (maximumLength <= 0)
        {
            throw new InvalidOperationException("Maximum length must be greater than zero.");
        }

        if (minimumLength > maximumLength)
        {
            throw new InvalidOperationException("Minimum length cannot be greater than maximum length.");
        }

        List<char> characters = [];
        AddRandomCharacters(characters, LowercaseCharacters, lowercaseCount);
        AddRandomCharacters(characters, UppercaseCharacters, uppercaseCount);
        AddRandomCharacters(characters, DigitCharacters, digitCount);
        AddRandomCharacters(characters, specialCharacters, specialCount);

        if (characters.Count == 0)
        {
            throw new InvalidOperationException("Password length must be greater than zero.");
        }

        if (characters.Count > maximumLength)
        {
            throw new InvalidOperationException("Required character counts exceed maximum length.");
        }

        int targetLength =
            RandomNumberGenerator.GetInt32(
                Math.Max(minimumLength, characters.Count),
                maximumLength + 1);

        string fillCharacters =
            LowercaseCharacters + UppercaseCharacters + DigitCharacters + specialCharacters;

        AddRandomCharacters(
            characters,
            fillCharacters,
            targetLength - characters.Count);

        ShuffleCharacters(characters);

        return new string([.. characters]);
    }

    private static void AddRandomCharacters(
        List<char> characters,
        string sourceCharacters,
        int count)
    {
        for (int i = 0; i < count; i++)
        {
            characters.Add(sourceCharacters[RandomNumberGenerator.GetInt32(sourceCharacters.Length)]);
        }
    }

    private static void ShuffleCharacters(List<char> characters)
    {
        for (int i = characters.Count - 1; i > 0; i--)
        {
            int swapIndex = RandomNumberGenerator.GetInt32(i + 1);
            (characters[i], characters[swapIndex]) = (characters[swapIndex], characters[i]);
        }
    }

    private static int ReadNonNegativeCount(string value, string fieldName)
    {
        if (!int.TryParse(value, out int count) || count < 0)
        {
            throw new InvalidOperationException($"{fieldName} count must be zero or greater.");
        }

        return count;
    }
}

public sealed record WebsiteCredentialApplyRequest(
    string CredentialKey,
    string Username,
    string Password);

public sealed record WebsiteCredentialImportRequest(
    string CredentialKey,
    string Url);
