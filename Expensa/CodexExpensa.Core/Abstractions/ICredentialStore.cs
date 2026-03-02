namespace CodexExpensa.Core.Abstractions;

/// <summary>
/// Stores secrets outside the database (Windows Credential Manager, Keychain, etc).
/// </summary>
public interface ICredentialStore
{
    bool TryGet(string key, out string? username, out string? password);

    void Save(string key, string username, string password);

    void Delete(string key);
}