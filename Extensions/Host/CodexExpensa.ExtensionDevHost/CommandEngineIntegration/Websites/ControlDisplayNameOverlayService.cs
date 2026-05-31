namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Websites;

public sealed class ControlDisplayNameOverlayService
{
    private readonly Dictionary<Control, string> _originalTextByControl = [];

    public void ShowControlNames(
        Control rootControl)
    {
        ArgumentNullException.ThrowIfNull(rootControl);

        foreach (Control control in EnumerateControls(rootControl))
        {
            if (!ShouldShowControlName(control))
            {
                continue;
            }

            if (!_originalTextByControl.ContainsKey(control))
            {
                _originalTextByControl[control] = control.Text;
            }

            control.Text =
                $"{_originalTextByControl[control]}  [{control.Name}]";
        }
    }

    public void HideControlNames(
        Control rootControl)
    {
        ArgumentNullException.ThrowIfNull(rootControl);

        foreach (Control control in EnumerateControls(rootControl))
        {
            if (_originalTextByControl.TryGetValue(control, out string? originalText))
            {
                control.Text = originalText;
            }
        }

        _originalTextByControl.Clear();
    }

    private static IEnumerable<Control> EnumerateControls(
        Control rootControl)
    {
        foreach (Control child in rootControl.Controls)
        {
            yield return child;

            foreach (Control descendant in EnumerateControls(child))
            {
                yield return descendant;
            }
        }
    }

    private static bool ShouldShowControlName(
        Control control)
    {
        if (string.IsNullOrWhiteSpace(control.Name))
        {
            return false;
        }

        if (control is TextBoxBase)
        {
            return false;
        }

        if (control is ListView)
        {
            return false;
        }

        if (control is NumericUpDown)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(control.Text))
        {
            return false;
        }

        return control is Label ||
            control is LinkLabel ||
            control is Button ||
            control is CheckBox ||
            control is GroupBox ||
            control is RadioButton;
    }
}
