using System.Windows.Forms;

namespace CodexExpensa.App.WinForms.UI.Budgets;

public sealed class BudgetTemplateForm : Form
{
    public BudgetTemplateForm()
    {
        Text = "Budget Template";
        FormBorderStyle = FormBorderStyle.None;

        var label = new Label
        {
            Dock = DockStyle.Fill,
            TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
            Text = "Budget Template\n\nPlaceholder screen.\nNext step: define template rows/categories and default values."
        };

        Controls.Add(label);
    }
}