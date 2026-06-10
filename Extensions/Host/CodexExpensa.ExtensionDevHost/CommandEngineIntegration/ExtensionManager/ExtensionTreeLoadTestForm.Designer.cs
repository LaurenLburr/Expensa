namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.ExtensionManager;

partial class ExtensionTreeLoadTestForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components is not null)
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        SuspendLayout();
        // 
        // ExtensionTreeLoadTestForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(900, 600);
        MinimizeBox = false;
        MaximizeBox = false;
        Name = "ExtensionTreeLoadTestForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Extension Tree Load Test";
        ResumeLayout(false);
    }
}
