using CodexExpensa.Core.Domain.Transactions;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CodexExpensa.App.WinForms.UI.Transactions;

public sealed class ChangeTransactionStatusForm : Form
{
    private readonly TransactionStatus _currentStatus;

    private readonly Label _lblCurrentStatus;
    private readonly ComboBox _cmbNewStatus;
    private readonly ComboBox _cmbReason;
    private readonly TextBox _txtReasonText;
    private readonly Button _btnOk;
    private readonly Button _btnCancel;

    public TransactionStatus SelectedStatus
    {
        get
        {
            if (_cmbNewStatus.SelectedItem is TransactionStatus status)
                return status;

            return _currentStatus;
        }
    }

    public TransactionChangeReason SelectedReason
    {
        get
        {
            if (_cmbReason.SelectedItem is TransactionChangeReason reason)
                return reason;

            return TransactionChangeReason.ManualCorrection;
        }
    }

    public string? ReasonText
    {
        get
        {
            string value = _txtReasonText.Text.Trim();
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }
    }

    public ChangeTransactionStatusForm(TransactionStatus currentStatus)
    {
        _currentStatus = currentStatus;

        Text = "Change Transaction Status";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(520, 230);

        _lblCurrentStatus = new Label
        {
            AutoSize = true,
            Location = new Point(20, 20),
            Text = $"Current Status: {currentStatus}"
        };

        Label lblNewStatus = new()
        {
            AutoSize = true,
            Location = new Point(20, 58),
            Text = "New Status:"
        };

        _cmbNewStatus = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(140, 54),
            Size = new Size(220, 24)
        };

        LoadAllowedStatuses(currentStatus);

        Label lblReason = new()
        {
            AutoSize = true,
            Location = new Point(20, 96),
            Text = "Reason:"
        };

        _cmbReason = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(140, 92),
            Size = new Size(220, 24)
        };

        foreach (TransactionChangeReason reason in Enum.GetValues<TransactionChangeReason>())
            _cmbReason.Items.Add(reason);

        _cmbReason.SelectedItem = TransactionChangeReason.ManualCorrection;

        Label lblReasonText = new()
        {
            AutoSize = true,
            Location = new Point(20, 134),
            Text = "Reason Text:"
        };

        _txtReasonText = new TextBox
        {
            Location = new Point(140, 130),
            Size = new Size(340, 24)
        };

        _btnOk = new Button
        {
            Text = "OK",
            Location = new Point(324, 180),
            Size = new Size(75, 28),
            DialogResult = DialogResult.None
        };
        _btnOk.Click += BtnOk_Click;

        _btnCancel = new Button
        {
            Text = "Cancel",
            Location = new Point(405, 180),
            Size = new Size(75, 28),
            DialogResult = DialogResult.Cancel
        };

        AcceptButton = _btnOk;
        CancelButton = _btnCancel;

        Controls.Add(_lblCurrentStatus);
        Controls.Add(lblNewStatus);
        Controls.Add(_cmbNewStatus);
        Controls.Add(lblReason);
        Controls.Add(_cmbReason);
        Controls.Add(lblReasonText);
        Controls.Add(_txtReasonText);
        Controls.Add(_btnOk);
        Controls.Add(_btnCancel);
    }

    private void LoadAllowedStatuses(TransactionStatus currentStatus)
    {
        _cmbNewStatus.Items.Clear();

        foreach (TransactionStatus status in TransactionStatusRules.GetAllowedTransitions(currentStatus))
            _cmbNewStatus.Items.Add(status);

        if (_cmbNewStatus.Items.Count > 0)
            _cmbNewStatus.SelectedIndex = 0;
    }

    private void BtnOk_Click(object? sender, EventArgs e)
    {
        if (_cmbNewStatus.Items.Count == 0)
        {
            MessageBox.Show(
                this,
                $"No valid status changes are available from {_currentStatus}.",
                "No Valid Transitions",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        if (_cmbNewStatus.SelectedItem is not TransactionStatus selectedStatus)
        {
            MessageBox.Show(
                this,
                "Please choose a new status.",
                "Status Required",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            _cmbNewStatus.Focus();
            return;
        }

        if (!TransactionStatusRules.IsValidTransition(_currentStatus, selectedStatus))
        {
            MessageBox.Show(
                this,
                $"Transition from {_currentStatus} to {selectedStatus} is not allowed.",
                "Invalid Transition",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            _cmbNewStatus.Focus();
            return;
        }

        if (_cmbReason.SelectedItem is not TransactionChangeReason)
        {
            MessageBox.Show(
                this,
                "Please choose a reason.",
                "Reason Required",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            _cmbReason.Focus();
            return;
        }

        if (SelectedStatus == TransactionStatus.Invalid && string.IsNullOrWhiteSpace(_txtReasonText.Text))
        {
            MessageBox.Show(
                this,
                "Please enter reason text when marking a transaction Invalid.",
                "Reason Text Required",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            _txtReasonText.Focus();
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }
}
