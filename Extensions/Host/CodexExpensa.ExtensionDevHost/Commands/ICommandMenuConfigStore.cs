namespace CodexExpensa.ExtensionDevHost.Commands;

public interface ICommandMenuConfigStore
{
    string FilePath { get; }

    CommandMenuConfigDocument Load();

    void Save(CommandMenuConfigDocument document);
}
