
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CodexExpensa.ExtensionDevHost.Commands;

namespace CodexExpensa.ExtensionDevHost.UI;

public sealed class CommandCatalogForm : Form
{
    private readonly CommandRegistry _registry;
    private readonly string _configPath;
    private readonly Action _rebuildMenu;

    private readonly TabControl _tabs;
    private readonly DataGridView _grid;
    private readonly TextBox _txtJson;

    private readonly TextBox _txtCommandKey;
    private readonly TextBox _txtTopLevelMenu;
    private readonly TextBox _txtMenuText;
    private readonly NumericUpDown _numMenuOrder;
    private readonly NumericUpDown _numItemOrder;
    private readonly Button _btnMenuUp;
    private readonly Button _btnMenuDown;
    private readonly Button _btnItemUp;
    private readonly Button _btnItemDown;
    private readonly Button _btnApplyDetails;

    private int _dragRowIndex = -1;
    private bool _loadingSelection;

    public CommandCatalogForm(CommandRegistry registry, string configPath, Action rebuildMenu)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        _configPath = configPath ?? throw new ArgumentNullException(nameof(configPath));
        _rebuildMenu = rebuildMenu ?? throw new ArgumentNullException(nameof(rebuildMenu));

        Text = "Command Catalog";
        Width = 1100;
        Height = 700;
        StartPosition = FormStartPosition.CenterParent;

        _tabs = new TabControl { Dock = DockStyle.Fill };

        var commandsTab = new TabPage("Commands");
        var detailsTab = new TabPage("Details");
        var jsonTab = new TabPage("JSON");

        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoGenerateColumns = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowDrop = true,
            MultiSelect = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect
        };

        _grid.MouseDown += Grid_MouseDown;
        _grid.MouseMove += Grid_MouseMove;
        _grid.DragOver += Grid_DragOver;
        _grid.DragDrop += Grid_DragDrop;
        _grid.SelectionChanged += Grid_SelectionChanged;

        var openJsonFolderButton = new Button { Text = "Open JSON Folder", AutoSize = true };
        openJsonFolderButton.Click += (_, _) => OpenJsonFolder();

        var commandsTopPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            WrapContents = false
        };
        commandsTopPanel.Controls.Add(openJsonFolderButton);

        commandsTab.Controls.Add(_grid);
        commandsTab.Controls.Add(commandsTopPanel);

        var detailsLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 7,
            Padding = new Padding(12),
            AutoSize = true
        };

        detailsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        detailsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        detailsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        detailsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

        _txtCommandKey = new TextBox { ReadOnly = true, Dock = DockStyle.Fill };
        _txtTopLevelMenu = new TextBox { ReadOnly = true, Dock = DockStyle.Fill };
        _txtMenuText = new TextBox { Dock = DockStyle.Fill };
        _numMenuOrder = new NumericUpDown { Minimum = 0, Maximum = 100000, Dock = DockStyle.Left, Width = 120 };
        _numItemOrder = new NumericUpDown { Minimum = 0, Maximum = 100000, Dock = DockStyle.Left, Width = 120 };

        _btnMenuUp = new Button { Text = "Menu Up", AutoSize = true };
        _btnMenuDown = new Button { Text = "Menu Down", AutoSize = true };
        _btnItemUp = new Button { Text = "Item Up", AutoSize = true };
        _btnItemDown = new Button { Text = "Item Down", AutoSize = true };
        _btnApplyDetails = new Button { Text = "Apply", AutoSize = true };

        _btnMenuUp.Click += (_, _) => MoveMenu(-1);
        _btnMenuDown.Click += (_, _) => MoveMenu(1);
        _btnItemUp.Click += (_, _) => MoveItem(-1);
        _btnItemDown.Click += (_, _) => MoveItem(1);
        _btnApplyDetails.Click += (_, _) => ApplyDetails();

        AddLabeledControl(detailsLayout, 0, "Command Key", _txtCommandKey);
        AddLabeledControl(detailsLayout, 1, "Top-Level Menu", _txtTopLevelMenu);
        AddLabeledControl(detailsLayout, 2, "Menu Text", _txtMenuText);
        AddLabeledControl(detailsLayout, 3, "Menu Order", _numMenuOrder);
        AddLabeledControl(detailsLayout, 4, "Item Order", _numItemOrder);

        var menuButtonsPanel = new FlowLayoutPanel { AutoSize = true, WrapContents = false };
        menuButtonsPanel.Controls.Add(_btnMenuUp);
        menuButtonsPanel.Controls.Add(_btnMenuDown);

        var itemButtonsPanel = new FlowLayoutPanel { AutoSize = true, WrapContents = false };
        itemButtonsPanel.Controls.Add(_btnItemUp);
        itemButtonsPanel.Controls.Add(_btnItemDown);

        detailsLayout.Controls.Add(new Label { Text = "Move Menu", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 5);
        detailsLayout.Controls.Add(menuButtonsPanel, 1, 5);
        detailsLayout.Controls.Add(new Label { Text = "Move Item", AutoSize = true, Anchor = AnchorStyles.Left }, 2, 5);
        detailsLayout.Controls.Add(itemButtonsPanel, 3, 5);

        var actionButtonsPanel = new FlowLayoutPanel { AutoSize = true, WrapContents = false };
        actionButtonsPanel.Controls.Add(_btnApplyDetails);

        detailsLayout.Controls.Add(new Label { Text = string.Empty, AutoSize = true }, 0, 6);
        detailsLayout.Controls.Add(actionButtonsPanel, 1, 6);

        detailsTab.Controls.Add(detailsLayout);

        _txtJson = new TextBox
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            ScrollBars = ScrollBars.Both,
            WordWrap = false
        };

        var reloadJsonButton = new Button { Text = "Reload JSON", AutoSize = true };
        var saveJsonButton = new Button { Text = "Save JSON", AutoSize = true };

        reloadJsonButton.Click += (_, _) => LoadJson();
        saveJsonButton.Click += (_, _) =>
        {
            File.WriteAllText(_configPath, _txtJson.Text);
            _registry.LoadConfig();
            _rebuildMenu();
            LoadGrid();
            LoadJson();
        };

        var jsonTopPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            WrapContents = false
        };
        jsonTopPanel.Controls.Add(reloadJsonButton);
        jsonTopPanel.Controls.Add(saveJsonButton);

        jsonTab.Controls.Add(_txtJson);
        jsonTab.Controls.Add(jsonTopPanel);

        _tabs.TabPages.Add(commandsTab);
        _tabs.TabPages.Add(detailsTab);
        _tabs.TabPages.Add(jsonTab);

        Controls.Add(_tabs);

        LoadGrid();
        LoadJson();
        LoadFirstRowIfAvailable();
    }

    private static void AddLabeledControl(TableLayoutPanel layout, int row, string labelText, Control control)
    {
        layout.Controls.Add(new Label
        {
            Text = labelText,
            AutoSize = true,
            Anchor = AnchorStyles.Left
        }, 0, row);

        layout.Controls.Add(control, 1, row);
        layout.SetColumnSpan(control, 3);
    }

    private void Grid_MouseDown(object? sender, MouseEventArgs e)
    {
        _dragRowIndex = _grid.HitTest(e.X, e.Y).RowIndex;
    }

    private void Grid_MouseMove(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left && _dragRowIndex >= 0)
        {
            _grid.DoDragDrop(_grid.Rows[_dragRowIndex], DragDropEffects.Move);
        }
    }

    private static void Grid_DragOver(object? sender, DragEventArgs e)
    {
        e.Effect = DragDropEffects.Move;
    }

    private void Grid_DragDrop(object? sender, DragEventArgs e)
    {
        List<Row> rows = GetRows();
        if (rows.Count == 0)
        {
            return;
        }

        var pt = _grid.PointToClient(new System.Drawing.Point(e.X, e.Y));
        int targetIndex = _grid.HitTest(pt.X, pt.Y).RowIndex;

        if (targetIndex < 0 || _dragRowIndex < 0 || _dragRowIndex >= rows.Count)
        {
            return;
        }

        if (targetIndex >= rows.Count)
        {
            targetIndex = rows.Count - 1;
        }

        Row moving = rows[_dragRowIndex];
        rows.RemoveAt(_dragRowIndex);
        rows.Insert(targetIndex, moving);

        RenumberAndPersistRows(rows);
        RebindGrid(rows, moving.CommandKey);
    }

    private void Grid_SelectionChanged(object? sender, EventArgs e)
    {
        if (_loadingSelection)
        {
            return;
        }

        Row? row = GetSelectedRow();
        if (row is null)
        {
            ClearDetails();
            return;
        }

        LoadDetails(row);
    }

    private void OpenJsonFolder()
    {
        string? folderPath = Path.GetDirectoryName(_configPath);
        if (string.IsNullOrWhiteSpace(folderPath))
        {
            MessageBox.Show(this, "JSON folder path is not available.", "Command Catalog", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        Process.Start(new ProcessStartInfo
        {
            FileName = folderPath,
            UseShellExecute = true
        });
    }

    private void LoadGrid()
    {
        List<Row> rows = _registry.GetCommands()
            .Select(x => new Row
            {
                CommandKey = x.CommandKey,
                MenuText = x.MenuText,
                MenuOrder = x.MenuOrder,
                ItemOrder = x.ItemOrder,
                TopLevelMenu = x.TopLevelMenu
            })
            .OrderBy(x => x.MenuOrder)
            .ThenBy(x => x.ItemOrder)
            .ThenBy(x => x.CommandKey, StringComparer.OrdinalIgnoreCase)
            .ToList();

        RebindGrid(rows, GetSelectedCommandKey());
    }

    private void LoadJson()
    {
        _txtJson.Text = File.Exists(_configPath)
            ? File.ReadAllText(_configPath)
            : "{ }";
    }

    private void LoadFirstRowIfAvailable()
    {
        if (_grid.Rows.Count > 0)
        {
            _grid.ClearSelection();
            _grid.Rows[0].Selected = true;
            if (_grid.CurrentCell is null && _grid.Rows[0].Cells.Count > 0)
            {
                _grid.CurrentCell = _grid.Rows[0].Cells[0];
            }
        }
    }

    private void LoadDetails(Row row)
    {
        _loadingSelection = true;
        try
        {
            _txtCommandKey.Text = row.CommandKey;
            _txtTopLevelMenu.Text = row.TopLevelMenu;
            _txtMenuText.Text = row.MenuText;
            _numMenuOrder.Value = Math.Max(_numMenuOrder.Minimum, Math.Min(_numMenuOrder.Maximum, row.MenuOrder));
            _numItemOrder.Value = Math.Max(_numItemOrder.Minimum, Math.Min(_numItemOrder.Maximum, row.ItemOrder));

            List<Row> rows = GetRows();
            List<string> menuNames = rows
                .Select(x => x.TopLevelMenu)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                .ToList();

            int currentMenuIndex = menuNames.FindIndex(x => string.Equals(x, row.TopLevelMenu, StringComparison.OrdinalIgnoreCase));
            List<Row> menuRows = rows
                .Where(x => string.Equals(x.TopLevelMenu, row.TopLevelMenu, StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x.ItemOrder)
                .ThenBy(x => x.CommandKey, StringComparer.OrdinalIgnoreCase)
                .ToList();
            int currentItemIndex = menuRows.FindIndex(x => string.Equals(x.CommandKey, row.CommandKey, StringComparison.OrdinalIgnoreCase));

            _btnMenuUp.Enabled = currentMenuIndex > 0;
            _btnMenuDown.Enabled = currentMenuIndex >= 0 && currentMenuIndex < menuNames.Count - 1;
            _btnItemUp.Enabled = currentItemIndex > 0;
            _btnItemDown.Enabled = currentItemIndex >= 0 && currentItemIndex < menuRows.Count - 1;
        }
        finally
        {
            _loadingSelection = false;
        }
    }

    private void ClearDetails()
    {
        _loadingSelection = true;
        try
        {
            _txtCommandKey.Clear();
            _txtTopLevelMenu.Clear();
            _txtMenuText.Clear();
            _numMenuOrder.Value = 0;
            _numItemOrder.Value = 0;

            _btnMenuUp.Enabled = false;
            _btnMenuDown.Enabled = false;
            _btnItemUp.Enabled = false;
            _btnItemDown.Enabled = false;
            _btnApplyDetails.Enabled = false;
        }
        finally
        {
            _loadingSelection = false;
        }
    }

    private void ApplyDetails()
    {
        Row? selected = GetSelectedRow();
        if (selected is null)
        {
            return;
        }

        _registry.UpdateMetadata(
            selected.CommandKey,
            _txtMenuText.Text,
            Decimal.ToInt32(_numMenuOrder.Value),
            Decimal.ToInt32(_numItemOrder.Value));

        _rebuildMenu();
        LoadGrid();
        LoadJson();
        SelectRowByCommandKey(selected.CommandKey);
        _tabs.SelectedIndex = 1;
    }

    private void MoveMenu(int direction)
    {
        Row? selected = GetSelectedRow();
        if (selected is null)
        {
            return;
        }

        List<Row> rows = GetRows();
        List<string> menuNames = rows
            .Select(x => x.TopLevelMenu)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x.MenuOrderFor(rows), Comparer<int>.Default)
            .ToList();

        int currentMenuIndex = menuNames.FindIndex(x => string.Equals(x, selected.TopLevelMenu, StringComparison.OrdinalIgnoreCase));
        if (currentMenuIndex < 0)
        {
            return;
        }

        int swapIndex = currentMenuIndex + direction;
        if (swapIndex < 0 || swapIndex >= menuNames.Count)
        {
            return;
        }

        string currentMenu = menuNames[currentMenuIndex];
        string swapMenu = menuNames[swapIndex];

        foreach (Row row in rows.Where(x => string.Equals(x.TopLevelMenu, currentMenu, StringComparison.OrdinalIgnoreCase)))
        {
            row.MenuOrder += direction < 0 ? -1000 : 1000;
        }

        foreach (Row row in rows.Where(x => string.Equals(x.TopLevelMenu, swapMenu, StringComparison.OrdinalIgnoreCase)))
        {
            row.MenuOrder += direction < 0 ? 1000 : -1000;
        }

        RenumberAndPersistRows(rows);
        RebindGrid(rows, selected.CommandKey);
        _tabs.SelectedIndex = 1;
    }

    private void MoveItem(int direction)
    {
        Row? selected = GetSelectedRow();
        if (selected is null)
        {
            return;
        }

        List<Row> rows = GetRows();
        List<Row> menuRows = rows
            .Where(x => string.Equals(x.TopLevelMenu, selected.TopLevelMenu, StringComparison.OrdinalIgnoreCase))
            .OrderBy(x => x.ItemOrder)
            .ThenBy(x => x.CommandKey, StringComparer.OrdinalIgnoreCase)
            .ToList();

        int currentIndex = menuRows.FindIndex(x => string.Equals(x.CommandKey, selected.CommandKey, StringComparison.OrdinalIgnoreCase));
        if (currentIndex < 0)
        {
            return;
        }

        int swapIndex = currentIndex + direction;
        if (swapIndex < 0 || swapIndex >= menuRows.Count)
        {
            return;
        }

        int currentItemOrder = menuRows[currentIndex].ItemOrder;
        menuRows[currentIndex].ItemOrder = menuRows[swapIndex].ItemOrder;
        menuRows[swapIndex].ItemOrder = currentItemOrder;

        RenumberAndPersistRows(rows);
        RebindGrid(rows, selected.CommandKey);
        _tabs.SelectedIndex = 1;
    }

    private void RenumberAndPersistRows(List<Row> rows)
    {
        int menuOrder = 100;

        foreach (var menuGroup in rows
                     .GroupBy(x => x.TopLevelMenu)
                     .OrderBy(g => g.Min(x => x.MenuOrder))
                     .ThenBy(g => g.Key, StringComparer.OrdinalIgnoreCase))
        {
            int itemOrder = 100;

            foreach (Row row in menuGroup
                         .OrderBy(x => x.ItemOrder)
                         .ThenBy(x => x.CommandKey, StringComparer.OrdinalIgnoreCase))
            {
                row.MenuOrder = menuOrder;
                row.ItemOrder = itemOrder;

                _registry.UpdateMetadata(
                    row.CommandKey,
                    row.MenuText,
                    row.MenuOrder,
                    row.ItemOrder);

                itemOrder += 100;
            }

            menuOrder += 100;
        }

        _rebuildMenu();
        LoadJson();
    }

    private void RebindGrid(List<Row> rows, string? selectedCommandKey)
    {
        _loadingSelection = true;
        try
        {
            _grid.DataSource = null;
            _grid.DataSource = rows
                .OrderBy(x => x.MenuOrder)
                .ThenBy(x => x.ItemOrder)
                .ThenBy(x => x.CommandKey, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
        finally
        {
            _loadingSelection = false;
        }

        if (!string.IsNullOrWhiteSpace(selectedCommandKey))
        {
            SelectRowByCommandKey(selectedCommandKey);
        }
        else
        {
            LoadFirstRowIfAvailable();
        }
    }

    private void SelectRowByCommandKey(string commandKey)
    {
        foreach (DataGridViewRow gridRow in _grid.Rows)
        {
            if (gridRow.DataBoundItem is Row row &&
                string.Equals(row.CommandKey, commandKey, StringComparison.OrdinalIgnoreCase))
            {
                _grid.ClearSelection();
                gridRow.Selected = true;
                if (gridRow.Cells.Count > 0)
                {
                    _grid.CurrentCell = gridRow.Cells[0];
                }

                LoadDetails(row);
                return;
            }
        }

        ClearDetails();
    }

    private Row? GetSelectedRow()
    {
        if (_grid.CurrentRow?.DataBoundItem is Row currentRow)
        {
            _btnApplyDetails.Enabled = true;
            return currentRow;
        }

        if (_grid.SelectedRows.Count > 0 &&
            _grid.SelectedRows[0].DataBoundItem is Row selectedRow)
        {
            _btnApplyDetails.Enabled = true;
            return selectedRow;
        }

        _btnApplyDetails.Enabled = false;
        return null;
    }

    private string? GetSelectedCommandKey()
    {
        return GetSelectedRow()?.CommandKey;
    }

    private List<Row> GetRows()
    {
        return _grid.DataSource as List<Row> ?? new List<Row>();
    }

    private sealed class Row
    {
        public string CommandKey { get; set; } = string.Empty;
        public string MenuText { get; set; } = string.Empty;
        public int MenuOrder { get; set; }
        public int ItemOrder { get; set; }
        public string TopLevelMenu { get; set; } = string.Empty;
    }
}

internal static class CommandCatalogRowExtensions
{
    public static int MenuOrderFor(this string topLevelMenu, IEnumerable<object> rows)
    {
        foreach (object row in rows)
        {
            if (row is null)
            {
                continue;
            }

            var rowType = row.GetType();
            var menuProperty = rowType.GetProperty("TopLevelMenu");
            var orderProperty = rowType.GetProperty("MenuOrder");
            if (menuProperty is null || orderProperty is null)
            {
                continue;
            }

            string? rowMenu = menuProperty.GetValue(row) as string;
            if (!string.Equals(rowMenu, topLevelMenu, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            object? orderValue = orderProperty.GetValue(row);
            if (orderValue is int menuOrder)
            {
                return menuOrder;
            }
        }

        return int.MaxValue;
    }
}
