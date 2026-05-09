namespace CodexExpensa.ExtensionDevHost.Commands;

public sealed class QueryCatalogMenuSpacerCommand : MenuSpacerCommand
{
    public override string CommandKey => "Tools.Spacer.100";

    public override string TopLevelMenu => "Tools";

    protected override int GetDefaultMenuOrder() => 300;

    protected override int GetDefaultItemOrder() => 100;
}
