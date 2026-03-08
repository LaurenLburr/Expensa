using System;
using System.Windows.Forms;
using CodexExpensa.Core.Domain.Payees;

namespace CodexExpensa.App.WinForms.UI.Payees;

public sealed class PayeeDetailsForm : Form
{
    private readonly IPayeeRepository _repo;
    private readonly Action _onSaved;

    private Payee? _payee;

    private readonly TextBox _txtPayeeName;
    private readonly CheckBox _chkTemplate;

    private readonly Button _btnSave;

    public PayeeDetailsForm(IPayeeRepository repo, Action onSaved)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        _onSaved = onSaved ?? throw new ArgumentNullException(nameof(onSaved));

        Text = "Payee";
        FormBorderStyle = FormBorderStyle.None;

        var root = new Panel { Dock = DockStyle.Fill, Padding = new Padding(12) };
        Controls.Add(root);

        var lblTitle = new Label { Text = "Payee Details", AutoSize = true, Left = 12, Top = 12 };
        root.Controls.Add(lblTitle);

        var lblName = new Label { Text = "Payee Name:", AutoSize = true, Left = 12, Top = 50 };
        _txtPayeeName = new TextBox
        {
            Left = 140,
            Top = 46,
            Width = 700,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        _chkTemplate = new CheckBox
        {
            Text = "Include in Budget Template",
            Left = 140,
            Top = 86,
            AutoSize = true
        };

        _btnSave = new Button
        {
            Text = "Save",
            Width = 100,
            Height = 30,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left
        };
        _btnSave.Click += (_, _) => Save();

        root.Controls.Add(lblName);
        root.Controls.Add(_txtPayeeName);
        root.Controls.Add(_chkTemplate);
        root.Controls.Add(_btnSave);

        root.Resize += (_, _) =>
        {
            _txtPayeeName.Width = root.Width - _txtPayeeName.Left - 24;
            _btnSave.Left = 140;
            _btnSave.Top = root.Height - _btnSave.Height - 12;
        };
    }

    public void LoadPayee(Payee payee)
    {
        _payee = payee ?? throw new ArgumentNullException(nameof(payee));

        _txtPayeeName.Text = payee.PayeeName;
        _chkTemplate.Checked = payee.IncludeInBudgetTemplate;
    }

    private void Save()
    {
        if (_payee is null)
            return;

        var name = _txtPayeeName.Text.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            MessageBox.Show(this, "Payee Name is required.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _txtPayeeName.Focus();
            return;
        }

        var updated = new Payee
        {
            PayeeId = _payee.PayeeId,
            PayeeName = name,
            IncludeInBudgetTemplate = _chkTemplate.Checked
        };

        try
        {
            _repo.Update(updated);
            _onSaved();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.ToString(), "Save failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}