using CodexExpensa.Core.Domain.Transactions;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CodexExpensa.App.WinForms.UI.Transactions;

public sealed class TransactionStatusHistoryForm : Form
{
    private readonly DataGridView _grid;

    public TransactionStatusHistoryForm(
        int transactionId,
        string transactionLabel,
        IReadOnlyList<TxnStatusLog> history)
    {
        if (transactionId <= 0)
            throw new ArgumentException("transactionId must be > 0.", nameof(transactionId));

        Text = $"Transaction Status History - {transactionLabel}";
        StartPosition = FormStartPosition.CenterParent;
        Width = 980;
        Height = 480;
        MinimizeBox = false;

        Label header = new()
        {
            Dock = DockStyle.Top,
            Height = 48,
            Padding = new Padding(10, 10, 10, 0),
            Text = $"TransactionId: {transactionId}    {transactionLabel}",
            AutoEllipsis = true
        };

        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoGenerateColumns = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToOrderColumns = false,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false
        };

        BuildColumns();
        _grid.DataSource = history;

        Controls.Add(_grid);
        Controls.Add(header);
    }

    private void BuildColumns()
    {
        _grid.Columns.Clear();

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "OldStatus",
            DataPropertyName = nameof(TxnStatusLog.OldStatus),
            HeaderText = "Old Status",
            Width = 110,
            ReadOnly = true
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "NewStatus",
            DataPropertyName = nameof(TxnStatusLog.NewStatus),
            HeaderText = "New Status",
            Width = 110,
            ReadOnly = true
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "ReasonCode",
            DataPropertyName = nameof(TxnStatusLog.ReasonCode),
            HeaderText = "Reason",
            Width = 160,
            ReadOnly = true
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "ReasonText",
            DataPropertyName = nameof(TxnStatusLog.ReasonText),
            HeaderText = "Reason Text",
            Width = 260,
            ReadOnly = true
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "ChangedUtc",
            DataPropertyName = nameof(TxnStatusLog.ChangedUtc),
            HeaderText = "Changed Utc",
            Width = 150,
            ReadOnly = true,
            DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy-MM-dd HH:mm:ss" }
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "ChangedBy",
            DataPropertyName = nameof(TxnStatusLog.ChangedBy),
            HeaderText = "Changed By",
            Width = 110,
            ReadOnly = true
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "Source",
            DataPropertyName = nameof(TxnStatusLog.Source),
            HeaderText = "Source",
            Width = 130,
            ReadOnly = true
        });
    }
}
