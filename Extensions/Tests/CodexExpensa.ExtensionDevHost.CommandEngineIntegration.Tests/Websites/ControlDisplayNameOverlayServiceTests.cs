using CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;
using Xunit;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Websites;

public sealed class ControlDisplayNameOverlayServiceTests
{
    [Fact]
    public void ShowControlNames_AppendsControlNameToDisplayableText()
    {
        using Form form = new();

        Label label = new()
        {
            Name = "demoLabel",
            Text = "Demo"
        };

        form.Controls.Add(label);

        ControlDisplayNameOverlayService service = new();

        service.ShowControlNames(form);

        Assert.Equal("Demo  [demoLabel]", label.Text);
    }

    [Fact]
    public void HideControlNames_RestoresOriginalText()
    {
        using Form form = new();

        Button button = new()
        {
            Name = "saveButton",
            Text = "Save"
        };

        form.Controls.Add(button);

        ControlDisplayNameOverlayService service = new();

        service.ShowControlNames(form);
        service.HideControlNames(form);

        Assert.Equal("Save", button.Text);
    }

    [Fact]
    public void ShowControlNames_DoesNotChangeTextBoxes()
    {
        using Form form = new();

        TextBox textBox = new()
        {
            Name = "pathTextBox",
            Text = "C:\\Temp\\demo.db"
        };

        form.Controls.Add(textBox);

        ControlDisplayNameOverlayService service = new();

        service.ShowControlNames(form);

        Assert.Equal("C:\\Temp\\demo.db", textBox.Text);
    }
}
