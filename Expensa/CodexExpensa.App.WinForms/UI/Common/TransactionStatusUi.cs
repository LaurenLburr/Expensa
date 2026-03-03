using System;
using System.ComponentModel;
using System.Windows.Forms;
using CodexExpensa.Core.Domain.Transactions;

namespace CodexExpensa.App.WinForms.UI.Common;

public static class TransactionStatusUi
{
    private static readonly BindingList<TransactionStatus> _dataSource =
        new((TransactionStatus[])Enum.GetValues(typeof(TransactionStatus)));

    public static BindingList<TransactionStatus> CreateDataSource() => _dataSource;

    public static DataGridViewComboBoxColumn CreateColumn(
        string dataPropertyName,
        string headerText = "Status",
        int width = 120)
    {
        if (string.IsNullOrWhiteSpace(dataPropertyName))
            throw new ArgumentException("Data property name is required.", nameof(dataPropertyName));

        return new DataGridViewComboBoxColumn
        {
            Name = dataPropertyName,
            HeaderText = headerText,
            DataPropertyName = dataPropertyName,
            DataSource = CreateDataSource(),
            ValueType = typeof(TransactionStatus),
            DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton,
            FlatStyle = FlatStyle.Flat,
            Width = width
        };
    }
}