using System;
using System.Windows.Forms;

namespace CodexExpensa.App.WinForms.UI.Budgets;

public sealed class BudgetMonthForm : Form
{
    private readonly int _year;
    private readonly int _month;

    public BudgetMonthForm(int year, int month)
    {
        _year = year;
        _month = month;

        Text = "Budget";
        FormBorderStyle = FormBorderStyle.None;

        var title = new Label
        {
            AutoSize = true,
            Left = 12,
            Top = 12,
            Text = $"Budget: {new DateTime(year, month, 1):yyyy-MMM}"
        };

        var hint = new Label
        {
            AutoSize = true,
            Left = 12,
            Top = 44,
            Text = "Placeholder screen.\nNext step: month-specific budget values, rollups, and linkage to transactions."
        };

        Controls.Add(title);
        Controls.Add(hint);
    }
}