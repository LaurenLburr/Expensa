namespace CodexExpensa.Navigation.Abstractions;

public interface ITreeNodeExtension
{
    string ExtensionKey { get; }
    int SortOrder { get; }
}
