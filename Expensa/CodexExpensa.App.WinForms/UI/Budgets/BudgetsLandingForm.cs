using System.Windows.Forms;

namespace CodexExpensa.App.WinForms.UI.Budgets;

public sealed class BudgetsLandingForm : Form
{
    public BudgetsLandingForm()
    {
        Text = "Budgets";
        FormBorderStyle = FormBorderStyle.None;

        var label = new Label
        {
            Dock = DockStyle.Fill,
            TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
            Text = "Budgets\n\nSelect Template or a Month to begin.\n(Budget support is next.)"
        };

        Controls.Add(label);
    }
}