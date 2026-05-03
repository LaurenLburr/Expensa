using System;
using System.Windows.Forms;

namespace CodexExpensa.ExtensionDevHost.Commands.Services;

public sealed class WinFormsCommandUiService : ICommandUiService
{
    public WinFormsCommandUiService(Form owner)
    {
        Owner = owner ?? throw new ArgumentNullException(nameof(owner));
    }

    public Form Owner { get; }

    public void Show(Form form)
    {
        if (form == null)
            throw new ArgumentNullException(nameof(form));

        form.Show(Owner);
    }

    public DialogResult ShowDialog(Form form)
    {
        if (form == null)
            throw new ArgumentNullException(nameof(form));

        return form.ShowDialog(Owner);
    }
}
