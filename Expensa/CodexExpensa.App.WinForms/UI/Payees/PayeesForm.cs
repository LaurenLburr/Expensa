using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using CodexExpensa.Core.Domain.Payees;

namespace CodexExpensa.App.WinForms.UI.Payees;

public sealed class PayeesForm : Form
{
    private readonly IPayeeRepository _repo;

    private readonly DataGridView _grid;
    private readonly Button _btnAdd;
    private readonly Button _btnDelete;
    private readonly Button _btnClose;

    private readonly BindingList<PayeeRow> _rows = new();

    public PayeesForm(IPayeeRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));

        Text = "Payees";
        StartPosition = FormStartPosition.CenterParent;
        Width = 520;
        Height = 420;

        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(12) };
        Controls.Add(panel);

        _grid = new DataGridView
        {
            Dock = DockStyle.Top,
            Height = 300,
            AutoGenerateColumns = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            DataSource = _rows
        };

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = nameof(PayeeRow.PayeeName),
            HeaderText = "Payee",
            DataPropertyName = nameof(PayeeRow.PayeeName),
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        });

        _btnAdd = new Button { Text = "Add", Width = 80, Height = 30, Left = 12, Top = 320 };
        _btnDelete = new Button { Text = "Delete", Width = 80, Height = 30, Left = 100, Top = 320 };
        _btnClose = new Button { Text = "Close", Width = 80, Height = 30, Left = 400, Top = 320, Anchor = AnchorStyles.Top | AnchorStyles.Right };

        _btnAdd.Click += (_, _) => AddPayee();
        _btnDelete.Click += (_, _) => DeleteSelected();
        _btnClose.Click += (_, _) => Close();

        panel.Controls.Add(_grid);
        panel.Controls.Add(_btnAdd);
        panel.Controls.Add(_btnDelete);
        panel.Controls.Add(_btnClose);

        Load += (_, _) => Reload();
        FormClosing += (_, _) => SaveEdits();
    }

    private void Reload()
    {
        _rows.Clear();
        foreach (var p in _repo.GetAll())
        {
            _rows.Add(new PayeeRow { PayeeId = p.PayeeId, PayeeName = p.PayeeName });
        }
    }

    private void AddPayee()
    {
        var name = Prompt.Show(this, "New Payee", "Payee name:");
        if (string.IsNullOrWhiteSpace(name))
            return;

        name = name.Trim();

        var existing = _repo.GetByName(name);
        if (existing is not null)
        {
            MessageBox.Show(this, "That payee already exists.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var payee = new Payee
        {
            PayeeId = Guid.NewGuid().ToString("N"),
            PayeeName = name
        };

        _repo.Add(payee);
        Reload();
    }

    private void DeleteSelected()
    {
        if (_grid.CurrentRow?.DataBoundItem is not PayeeRow row)
            return;

        var ConfirmationNumber = MessageBox.Show(this, $"Delete payee:\n\n{row.PayeeName}\n\nTransactions will keep the PayeeId NULL.",
            "Delete Payee", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

        if (ConfirmationNumber != DialogResult.Yes)
            return;

        _repo.Delete(row.PayeeId);
        Reload();
    }

    private void SaveEdits()
    {
        // Persist edited names.
        // (Simple approach: update changed rows.)
        foreach (var row in _rows)
        {
            if (string.IsNullOrWhiteSpace(row.PayeeId))
                continue;

            if (string.IsNullOrWhiteSpace(row.PayeeName))
                continue;

            var current = _repo.GetById(row.PayeeId);
            if (current is null)
                continue;

            var trimmed = row.PayeeName.Trim();
            if (!string.Equals(current.PayeeName, trimmed, StringComparison.Ordinal))
            {
                _repo.Update(new Payee { PayeeId = row.PayeeId, PayeeName = trimmed });
            }
        }
    }

    private sealed class PayeeRow
    {
        public string PayeeId { get; set; } = string.Empty;
        public string PayeeName { get; set; } = string.Empty;
    }

    private static class Prompt
    {
        public static string? Show(IWin32Window owner, string title, string label)
        {
            using var f = new Form
            {
                Text = title,
                Width = 420,
                Height = 160,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var lbl = new Label { Left = 12, Top = 12, AutoSize = true, Text = label };
            var txt = new TextBox { Left = 12, Top = 40, Width = 380 };

            var ok = new Button { Text = "OK", Left = 232, Width = 75, Top = 75, DialogResult = DialogResult.OK };
            var cancel = new Button { Text = "Cancel", Left = 317, Width = 75, Top = 75, DialogResult = DialogResult.Cancel };

            f.Controls.Add(lbl);
            f.Controls.Add(txt);
            f.Controls.Add(ok);
            f.Controls.Add(cancel);

            f.AcceptButton = ok;
            f.CancelButton = cancel;

            return f.ShowDialog(owner) == DialogResult.OK ? txt.Text : null;
        }
    }
}