using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Forms = System.Windows.Forms;
using Drawing = System.Drawing;

namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Budgets;

public sealed partial class BudgetTransactionWpfGridControl : System.Windows.Controls.UserControl
{
    private const string AmountColumnName = "Amount";
    private const string AmountPaidColumnName = "Amt. Paid";

    private static readonly HashSet<string> HiddenColumns =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "RowType",
            "NodeId",
            "ParentNodeId",
            "HierarchyLevel",
            "SortIndex",
            "BudgetMonthRowId",
            "PayeeId",
            "TransactionId"
        };

    private readonly List<BudgetTransactionWpfGridRow> _sourceRows = [];
    private readonly ObservableCollection<BudgetTransactionWpfGridRow> _visibleRows = [];
    private IReadOnlyList<string> _displayColumns = [];

    public event EventHandler<BudgetTransactionWpfGridRow>? RowContextMenuRequested;

    public event EventHandler<BudgetTransactionWpfGridRow>? TransactionRowDoubleClicked;

    public BudgetTransactionWpfGridControl()
    {
        InitializeComponent();
        BudgetRowsDataGrid.ItemsSource = _visibleRows;
    }

    public BudgetTransactionWpfGridRow? SelectedRow =>
        BudgetRowsDataGrid.SelectedItem as BudgetTransactionWpfGridRow;

    public void LoadRows(DataTable? rows)
    {
        _sourceRows.Clear();
        _displayColumns = rows is null
            ? []
            : rows.Columns
                .Cast<DataColumn>()
                .Select(static column => column.ColumnName)
                .Where(static column => !HiddenColumns.Contains(column))
                .Aggregate(
                    new List<string>(),
                    static (columns, columnName) =>
                    {
                        columns.Add(columnName);

                        if (string.Equals(
                                columnName,
                                AmountColumnName,
                                StringComparison.OrdinalIgnoreCase))
                        {
                            columns.Add(AmountPaidColumnName);
                        }

                        return columns;
                    })
                .ToArray();

        if (rows is not null)
        {
            foreach (DataRow row in rows.Rows)
            {
                Dictionary<string, object?> values =
                    new(StringComparer.OrdinalIgnoreCase);

                foreach (DataColumn column in rows.Columns)
                {
                    object? value =
                        row[column] is DBNull
                            ? null
                            : row[column];

                    values[column.ColumnName] = value;
                }

                _sourceRows.Add(new BudgetTransactionWpfGridRow(values));
            }
        }

        RebuildColumns();
        RenderRows();
    }

    private void RebuildColumns()
    {
        BudgetRowsDataGrid.Columns.Clear();

        foreach (string columnName in _displayColumns)
        {
            if (string.Equals(
                    columnName,
                    "Item",
                    StringComparison.OrdinalIgnoreCase))
            {
                BudgetRowsDataGrid.Columns.Add(CreateItemColumn(columnName));
                continue;
            }

            BudgetRowsDataGrid.Columns.Add(
                new DataGridTextColumn
                {
                    Header = columnName,
                    Binding = new System.Windows.Data.Binding($"Values[{columnName}]")
                    {
                        ConverterCulture = CultureInfo.InvariantCulture
                    },
                    IsReadOnly = true
                });
        }
    }

    private DataGridTemplateColumn CreateItemColumn(string columnName)
    {
        return new DataGridTemplateColumn
        {
            Header = columnName,
            CellTemplate = (DataTemplate)FindResource("ItemCellTemplate")
        };
    }

    private void ToggleButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not System.Windows.Controls.Button button ||
            button.DataContext is not BudgetTransactionWpfGridRow row ||
            string.IsNullOrWhiteSpace(row.ToggleText))
        {
            return;
        }

        ToggleBudgetRow(row);
        e.Handled = true;
    }

    private void ToggleStatusColorPanel_Click(object sender, RoutedEventArgs e)
    {
        bool shouldShow =
            StatusColorPanel.Visibility != Visibility.Visible;

        StatusColorPanel.Visibility = Visibility.Visible;

        double from =
            shouldShow
                ? 36
                : 0;
        double to =
            shouldShow
                ? 0
                : 36;

        DoubleAnimation animation =
            new(from, to, TimeSpan.FromMilliseconds(160))
            {
                FillBehavior = FillBehavior.Stop
            };

        animation.Completed += (_, _) =>
        {
            if (!shouldShow)
            {
                StatusColorPanel.Visibility = Visibility.Collapsed;
            }

            if (StatusColorPanel.RenderTransform is TranslateTransform transform)
            {
                transform.Y = to;
            }
        };

        if (StatusColorPanel.RenderTransform is TranslateTransform panelTransform)
        {
            panelTransform.BeginAnimation(TranslateTransform.YProperty, animation);
        }

        e.Handled = true;
    }

    private void StatusColorSwatch_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement element ||
            element.Tag is not string brushKey)
        {
            return;
        }

        PickStatusColor(brushKey);
        e.Handled = true;
    }

    private void PickStatusColor(string brushKey)
    {
        if (FindResource(brushKey) is not SolidColorBrush brush)
        {
            return;
        }

        using Forms.ColorDialog dialog =
            new()
            {
                Color = Drawing.Color.FromArgb(
                    brush.Color.A,
                    brush.Color.R,
                    brush.Color.G,
                    brush.Color.B),
                FullOpen = true
            };

        if (dialog.ShowDialog() != Forms.DialogResult.OK)
        {
            return;
        }

        Resources[brushKey] =
            new SolidColorBrush(
                System.Windows.Media.Color.FromArgb(
                    dialog.Color.A,
                    dialog.Color.R,
                    dialog.Color.G,
                    dialog.Color.B));

        BudgetRowsDataGrid.Items.Refresh();
        BudgetRowsDataGrid.UpdateLayout();
    }

    private static T? FindParent<T>(DependencyObject child)
        where T : DependencyObject
    {
        DependencyObject? current = child;

        while (current is not null)
        {
            if (current is T match)
            {
                return match;
            }

            current = System.Windows.Media.VisualTreeHelper.GetParent(current);
        }

        return null;
    }

    private void Grid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (SelectedRow is not BudgetTransactionWpfGridRow row)
        {
            return;
        }

        if (row.IsTransactionHeader)
        {
            return;
        }

        if (row.IsTransaction)
        {
            TransactionRowDoubleClicked?.Invoke(this, row);
            return;
        }

        ToggleBudgetRow(row);
    }

    private void TransactionGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is not DataGrid grid ||
            grid.SelectedItem is not BudgetTransactionWpfGridRow row)
        {
            return;
        }

        TransactionRowDoubleClicked?.Invoke(this, row);
        e.Handled = true;
    }

    private void StatusContextMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem menuItem ||
            menuItem.Tag is not string status ||
            menuItem.DataContext is not BudgetTransactionWpfGridRow row)
        {
            return;
        }

        row.Status = status;
        e.Handled = true;
    }

    private void Grid_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        DependencyObject? source =
            e.OriginalSource as DependencyObject;

        DataGridRow? row =
            FindParent<DataGridRow>(source!);
        DataGridCell? cell =
            FindParent<DataGridCell>(source!);

        if (!IsContextMenuCell(cell) ||
            row?.Item is not BudgetTransactionWpfGridRow budgetRow)
        {
            return;
        }

        if (budgetRow.IsTransactionHeader)
        {
            return;
        }

        BudgetRowsDataGrid.SelectedItem = budgetRow;
        RowContextMenuRequested?.Invoke(this, budgetRow);
        e.Handled = true;
    }

    private void TransactionGrid_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        DependencyObject? source =
            e.OriginalSource as DependencyObject;

        DataGridRow? row =
            FindParent<DataGridRow>(source!);
        DataGridCell? cell =
            FindParent<DataGridCell>(source!);

        if (!IsContextMenuCell(cell) ||
            row?.Item is not BudgetTransactionWpfGridRow transactionRow)
        {
            return;
        }

        if (sender is DataGrid grid)
        {
            grid.SelectedItem = transactionRow;
        }

        RowContextMenuRequested?.Invoke(this, transactionRow);
        e.Handled = true;
    }

    private static bool IsContextMenuCell(DataGridCell? cell)
    {
        if (cell is null)
        {
            return false;
        }

        return cell.Column.DisplayIndex == 0;
    }

    private void ToggleBudgetRow(BudgetTransactionWpfGridRow row)
    {
        if (row.IsTransaction ||
            !row.HasTransactions)
        {
            return;
        }

        row.IsExpanded = !row.IsExpanded;
        row.ToggleText = row.IsExpanded ? "-" : "+";
    }

    private void RenderRows()
    {
        _visibleRows.Clear();

        IReadOnlyDictionary<string, List<BudgetTransactionWpfGridRow>> transactionsByBudgetRowId =
            _sourceRows
                .Where(static row => row.IsTransaction)
                .GroupBy(
                    static row => row.BudgetMonthRowId,
                    StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    static group => group.Key,
                    static group => group.ToList(),
                    StringComparer.OrdinalIgnoreCase);

        foreach (BudgetTransactionWpfGridRow row in _sourceRows)
        {
            row.Transactions.Clear();

            if (row.IsTransaction)
            {
                continue;
            }

            if (transactionsByBudgetRowId.TryGetValue(row.BudgetMonthRowId, out List<BudgetTransactionWpfGridRow>? transactions))
            {
                foreach (BudgetTransactionWpfGridRow transaction in transactions)
                {
                    row.Transactions.Add(transaction);
                }
            }

            row.IsExpanded = row.HasTransactions;
            row.ToggleText = row.HasTransactions
                ? row.IsExpanded ? "-" : "+"
                : string.Empty;
            row.DisplayItem = row.Item;
            row.RollupStatus = GetBudgetRollupStatus(row);
            row.SetValue(
                AmountPaidColumnName,
                FormatAmount(
                    row.Transactions.Sum(static transaction =>
                        transaction.GetDecimal(AmountColumnName))));

            _visibleRows.Add(row);
        }
    }

    private static string FormatAmount(decimal amount) =>
        amount == 0m
            ? string.Empty
            : amount.ToString("0.##", CultureInfo.InvariantCulture);

    private static string GetBudgetRollupStatus(BudgetTransactionWpfGridRow row)
    {
        if (!row.HasTransactions)
        {
            return string.Empty;
        }

        if (row.Transactions.Any(static transaction =>
                string.Equals(
                    transaction.GetValue("Status"),
                    "Outstanding",
                    StringComparison.OrdinalIgnoreCase)))
        {
            return "Outstanding";
        }

        return row.Transactions.All(static transaction =>
            string.Equals(
                transaction.GetValue("Status"),
                "Cleared",
                StringComparison.OrdinalIgnoreCase))
            ? "Cleared"
            : string.Empty;
    }
}

public sealed class BudgetTransactionWpfGridRow : INotifyPropertyChanged
{
    private bool _isExpanded;
    private string _rollupStatus = string.Empty;

    public BudgetTransactionWpfGridRow(IReadOnlyDictionary<string, object?> values)
    {
        Values =
            new Dictionary<string, object?>(
                values,
                StringComparer.OrdinalIgnoreCase);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public Dictionary<string, object?> Values { get; }

    public ObservableCollection<BudgetTransactionWpfGridRow> Transactions { get; } = [];

    public bool HasTransactions =>
        Transactions.Count > 0;

    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (_isExpanded == value)
            {
                return;
            }

            _isExpanded = value;
            OnPropertyChanged();
        }
    }

    public string RollupStatus
    {
        get => _rollupStatus;
        set
        {
            if (string.Equals(_rollupStatus, value, StringComparison.Ordinal))
            {
                return;
            }

            _rollupStatus = value;
            OnPropertyChanged();
        }
    }

    public string ToggleText
    {
        get => GetValue("__ToggleText");
        set
        {
            Values["__ToggleText"] = value;
            OnPropertyChanged();
        }
    }

    public string DisplayItem
    {
        get => GetValue("Item");
        set
        {
            Values["Item"] = value;
            OnPropertyChanged();
        }
    }

    public string Item =>
        GetValue("Item");

    public string Status
    {
        get => GetValue("Status");
        set
        {
            if (string.Equals(Status, value, StringComparison.Ordinal))
            {
                return;
            }

            Values["Status"] = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Values));
        }
    }

    public string BudgetMonthRowId =>
        GetValue("BudgetMonthRowId");

    public bool IsTransaction =>
        string.Equals(GetValue("RowType"), "Transaction", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(GetValue("RowType"), "Child", StringComparison.OrdinalIgnoreCase);

    public bool IsTransactionHeader =>
        string.Equals(GetValue("RowType"), "TransactionHeader", StringComparison.OrdinalIgnoreCase);

    public string GetValue(string columnName)
    {
        return Values.TryGetValue(columnName, out object? value)
            ? Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty
            : string.Empty;
    }

    public void SetValue(string columnName, object? value)
    {
        Values[columnName] = value;
        OnPropertyChanged(nameof(Values));
    }

    public decimal GetDecimal(string columnName)
    {
        string value =
            GetValue(columnName);

        return decimal.TryParse(
            value,
            NumberStyles.Number,
            CultureInfo.InvariantCulture,
            out decimal amount)
            ? amount
            : 0m;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
