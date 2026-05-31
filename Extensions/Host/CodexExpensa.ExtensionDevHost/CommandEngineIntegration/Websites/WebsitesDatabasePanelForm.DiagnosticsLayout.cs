namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed partial class WebsitesDatabasePanelForm
{
    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);

        PositionDiagnosticsToolStrip();
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);

        PositionDiagnosticsToolStrip();
    }

    private void PositionDiagnosticsToolStrip()
    {
        if (diagnosticsToolStrip is null)
        {
            return;
        }

        diagnosticsToolStrip.Location = new Point(
            Math.Max(0, ClientSize.Width - diagnosticsToolStrip.Width - 8),
            8);

        diagnosticsToolStrip.BringToFront();
    }
}
