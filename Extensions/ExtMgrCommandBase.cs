using System.Windows.Forms;

namespace CodexExpensa.ExtensionDevHost.Commands;

public abstract class ExtMgrCommandBase : IExtMgrCommand
{
    private string? _menuTextOverride;
    private int? _menuOrderOverride;
    private int? _itemOrderOverride;

    public abstract string CommandKey { get; }

    public abstract string TopLevelMenu { get; }

    public string MenuText => _menuTextOverride ?? GetDefaultMenuText();

    public int MenuOrder => _menuOrderOverride ?? GetDefaultMenuOrder();

    public int ItemOrder => _itemOrderOverride ?? GetDefaultItemOrder();

    public virtual bool IsSeparator => false;

    protected abstract string GetDefaultMenuText();

    protected virtual int GetDefaultMenuOrder() => 1000;

    protected virtual int GetDefaultItemOrder() => 1000;

    public void SetMetadata(string menuText, int menuOrder, int itemOrder)
    {
        _menuTextOverride = string.IsNullOrWhiteSpace(menuText) ? GetDefaultMenuText() : menuText.Trim();
        _menuOrderOverride = menuOrder;
        _itemOrderOverride = itemOrder;
    }

    public CommandMenuConfigEntry ToConfigEntry()
    {
        return new CommandMenuConfigEntry
        {
            CommandKey = CommandKey,
            MenuText = MenuText,
            MenuOrder = MenuOrder,
            ItemOrder = ItemOrder
        };
    }

    protected static void ShowNotAvailable(Form owner, string featureName)
    {
        MessageBox.Show(
            owner,
            $"{featureName} is not available in the current build.",
            "Extension Manager",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    protected static Form? CreateFormIfAvailable(Form owner, string fullTypeName, params object[] args)
    {
        System.Type? type = owner.GetType().Assembly.GetType(fullTypeName, throwOnError: false, ignoreCase: false);
        if (type is null || !typeof(Form).IsAssignableFrom(type))
        {
            return null;
        }

        object? instance = System.Activator.CreateInstance(type, args);
        return instance as Form;
    }

    public abstract void Execute(Form owner);
}
