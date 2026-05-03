
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
    private bool _loadingSelection;
    private int _dragRowIndex = -1;

    public CommandCatalogForm(CommandRegistry registry, string configPath, Action rebuildMenu)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        _configPath = configPath ?? throw new ArgumentNullException(nameof(configPath));
        _rebuildMenu = rebuildMenu ?? throw new ArgumentNullException(nameof(rebuildMenu));

        Text = "Command Catalog";
        Width = 1100;
        Height = 700;
        StartPosition = FormStartPosition.CenterParent;

        var tabs = new TabControl { Dock = DockStyle.Fill };

        var commandsTab = new TabPage("Commands");
        var detailsTab = new TabPage("Details");
        var jsonTab = new TabPage("JSON");

        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoGenerateColumns = true,
            AllowUserToAddRows = false,
            AllowDrop = true,
            ReadOnly = true,
            MultiSelect = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect
        };

        _grid.MouseDown += Grid_MouseDown;
        _grid.MouseMove += Grid_MouseMove;
        _grid.DragOver += (_, e) => e.Effect = DragDropEffects.Move;
        _grid.DragDrop += Grid_DragDrop;
        _grid.SelectionChanged += Grid_SelectionChanged;

        var openBtn = new Button { Text = "Open JSON Folder", AutoSize = true };
        openBtn.Click += (_, _) => OpenJsonFolder();

        var refreshBtn = new Button { Text = "Refresh", AutoSize = true };
        refreshBtn.Click += (_, _) => RefreshAll();

        var topPanel = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true };
        topPanel.Controls.Add(openBtn);
        topPanel.Controls.Add(refreshBtn);

        commandsTab.Controls.Add(_grid);
        commandsTab.Controls.Add(topPanel);

        var detailsPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            ColumnCount = 4,
            RowCount = 7,
            Padding = new Padding(12)
        };
        detailsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        detailsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        detailsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        detailsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        _txtCommandKey = new TextBox { ReadOnly = true, Dock = DockStyle.Fill };
        _txtTopLevelMenu = new TextBox { ReadOnly = true, Dock = DockStyle.Fill };
        _txtMenuText = new TextBox { Dock = DockStyle.Fill };
        _numMenuOrder = new NumericUpDown { Minimum = 0, Maximum = 100000, Dock = DockStyle.Left, Width = 120 };
        _numItemOrder = new NumericUpDown { Minimum = 0, Maximum = 100000, Dock = DockStyle.Left, Width = 120 };

        _btnMenuUp = new Button { Text = "Up", AutoSize = true };
        _btnMenuDown = new Button { Text = "Down", AutoSize = true };
        _btnItemUp = new Button { Text = "Up", AutoSize = true };
        _btnItemDown = new Button { Text = "Down", AutoSize = true };

        _btnMenuUp.Click += (_, _) => MoveMenuGroup(-1);
        _btnMenuDown.Click += (_, _) => MoveMenuGroup(1);
        _btnItemUp.Click += (_, _) => MoveItem(-1);
        _btnItemDown.Click += (_, _) => MoveItem(1);

        _txtMenuText.Leave += (_, _) => SaveDetailsIfNeeded();
        _numMenuOrder.Leave += (_, _) => SaveDetailsIfNeeded();
        _numItemOrder.Leave += (_, _) => SaveDetailsIfNeeded();
        _numMenuOrder.ValueChanged += (_, _) => { if (!_loadingSelection) SaveDetailsIfNeeded(); };
        _numItemOrder.ValueChanged += (_, _) => { if (!_loadingSelection) SaveDetailsIfNeeded(); };

        AddRow(detailsPanel, 0, "Command Key", _txtCommandKey);
        AddRow(detailsPanel, 1, "Top-Level Menu", _txtTopLevelMenu);
        AddRow(detailsPanel, 2, "Menu Text", _txtMenuText);
        AddOrderRow(detailsPanel, 3, "Menu Order", _numMenuOrder, _btnMenuUp, _btnMenuDown);
        AddOrderRow(detailsPanel, 4, "Item Order", _numItemOrder, _btnItemUp, _btnItemDown);

        detailsTab.Controls.Add(detailsPanel);

        _txtJson = new TextBox
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            ScrollBars = ScrollBars.Both,
            WordWrap = false
        };

        var reloadJsonBtn = new Button { Text = "Reload JSON", AutoSize = true };
        var saveJsonBtn = new Button { Text = "Save JSON", AutoSize = true };
        reloadJsonBtn.Click += (_, _) => LoadJson();
        saveJsonBtn.Click += (_, _) =>
        {
            File.WriteAllText(_configPath, _txtJson.Text);
            _registry.LoadConfig();
            _registry.NormalizeAndSave();
            _rebuildMenu();
            RefreshAll();
        };

        var jsonPanel = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true };
        jsonPanel.Controls.Add(reloadJsonBtn);
        jsonPanel.Controls.Add(saveJsonBtn);

        jsonTab.Controls.Add(_txtJson);
        jsonTab.Controls.Add(jsonPanel);

        tabs.TabPages.Add(commandsTab);
        tabs.TabPages.Add(detailsTab);
        tabs.TabPages.Add(jsonTab);

        Controls.Add(tabs);

        RefreshAll();
    }

    private void AddRow(TableLayoutPanel panel, int rowIndex, string labelText, Control editor)
    {
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        panel.Controls.Add(new Label { Text = labelText, AutoSize = true, Anchor = AnchorStyles.Left }, 0, rowIndex);
        panel.Controls.Add(editor, 1, rowIndex);
        panel.SetColumnSpan(editor, 3);
    }

    private void AddOrderRow(TableLayoutPanel panel, int rowIndex, string labelText, Control editor, Button upButton, Button downButton)
    {
        panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        panel.Controls.Add(new Label { Text = labelText, AutoSize = true, Anchor = AnchorStyles.Left }, 0, rowIndex);
        panel.Controls.Add(editor, 1, rowIndex);
        panel.Controls.Add(upButton, 2, rowIndex);
        panel.Controls.Add(downButton, 3, rowIndex);
    }

    private void RefreshAll()
    {
        LoadGrid();
        LoadJson();
        UpdateButtons();
    }

    private void LoadGrid()
    {
        var rows = _registry.GetCommands()
            .Select(static x => new Row
            {
                CommandKey = x.CommandKey,
                TopLevelMenu = x.TopLevelMenu,
                MenuText = x.MenuText,
                MenuOrder = x.MenuOrder,
                ItemOrder = x.ItemOrder
            })
            .OrderBy(static x => x.MenuOrder)
            .ThenBy(static x => x.TopLevelMenu, StringComparer.OrdinalIgnoreCase)
            .ThenBy(static x => x.ItemOrder)
            .ThenBy(static x => x.MenuText, StringComparer.OrdinalIgnoreCase)
            .ToList();

        string? selectedCommandKey = GetSelectedCommandKey();

        _grid.DataSource = null;
        _grid.DataSource = rows;

        foreach (DataGridViewColumn column in _grid.Columns)
        {
            column.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        SelectRowByCommandKey(selectedCommandKey ?? rows.FirstOrDefault()?.CommandKey);
    }

    private void LoadJson()
    {
        _txtJson.Text = File.Exists(_configPath) ? File.ReadAllText(_configPath) : "{ }";
    }

    private void OpenJsonFolder()
    {
        string? folder = Path.GetDirectoryName(_configPath);
        if (string.IsNullOrWhiteSpace(folder))
        {
            return;
        }

        Directory.CreateDirectory(folder);

        Process.Start(new ProcessStartInfo
        {
            FileName = folder,
            UseShellExecute = true
        });
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

    private void Grid_DragDrop(object? sender, DragEventArgs e)
    {
        if (_grid.DataSource is not List<Row> rows)
        {
            return;
        }

        var pt = _grid.PointToClient(new System.Drawing.Point(e.X, e.Y));
        int targetIndex = _grid.HitTest(pt.X, pt.Y).RowIndex;

        if (targetIndex < 0 || _dragRowIndex < 0 || _dragRowIndex >= rows.Count || targetIndex >= rows.Count)
        {
            return;
        }

        Row dragged = rows[_dragRowIndex];
        Row target = rows[targetIndex];

        if (!string.Equals(dragged.TopLevelMenu, target.TopLevelMenu, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        rows.RemoveAt(_dragRowIndex);
        rows.Insert(targetIndex, dragged);

        RenumberRows(rows);
        ApplyRows(rows, dragged.CommandKey);
    }

    private void Grid_SelectionChanged(object? sender, EventArgs e)
    {
        LoadDetailsFromSelection();
    }

    private string? GetSelectedCommandKey()
    {
        if (_grid.CurrentRow?.DataBoundItem is Row row)
        {
            return row.CommandKey;
        }

        return null;
    }

    private void SelectRowByCommandKey(string? commandKey)
    {
        if (string.IsNullOrWhiteSpace(commandKey))
        {
            LoadDetails(null);
            return;
        }

        foreach (DataGridViewRow row in _grid.Rows)
        {
            if (row.DataBoundItem is Row bound &&
                string.Equals(bound.CommandKey, commandKey, StringComparison.OrdinalIgnoreCase))
            {
                row.Selected = true;
                _grid.CurrentCell = row.Cells[0];
                LoadDetails(bound);
                return;
            }
        }

        LoadDetails(null);
    }

    private void LoadDetailsFromSelection()
    {
        if (_grid.CurrentRow?.DataBoundItem is Row row)
        {
            LoadDetails(row);
            return;
        }

        LoadDetails(null);
    }

    private void LoadDetails(Row? row)
    {
        _loadingSelection = true;

        if (row is null)
        {
            _txtCommandKey.Text = string.Empty;
            _txtTopLevelMenu.Text = string.Empty;
            _txtMenuText.Text = string.Empty;
            _numMenuOrder.Value = 0;
            _numItemOrder.Value = 0;
            UpdateButtons();
            _loadingSelection = false;
            return;
        }

        _txtCommandKey.Text = row.CommandKey;
        _txtTopLevelMenu.Text = row.TopLevelMenu;
        _txtMenuText.Text = row.MenuText;
        _numMenuOrder.Value = ClampNumeric(_numMenuOrder, row.MenuOrder);
        _numItemOrder.Value = ClampNumeric(_numItemOrder, row.ItemOrder);

        _loadingSelection = false;
        UpdateButtons();
    }

    private decimal ClampNumeric(NumericUpDown control, int value)
    {
        decimal decimalValue = value;
        if (decimalValue < control.Minimum)
        {
            return control.Minimum;
        }

        if (decimalValue > control.Maximum)
        {
            return control.Maximum;
        }

        return decimalValue;
    }

    private void SaveDetailsIfNeeded()
    {
        if (_loadingSelection)
        {
            return;
        }

        if (_grid.DataSource is not List<Row> rows)
        {
            return;
        }

        string? commandKey = _txtCommandKey.Text;
        if (string.IsNullOrWhiteSpace(commandKey))
        {
            return;
        }

        Row? row = rows.FirstOrDefault(x => string.Equals(x.CommandKey, commandKey, StringComparison.OrdinalIgnoreCase));
        if (row is null)
        {
            return;
        }

        row.MenuText = _txtMenuText.Text.Trim();
        row.MenuOrder = decimal.ToInt32(_numMenuOrder.Value);
        row.ItemOrder = decimal.ToInt32(_numItemOrder.Value);

        RenumberRows(rows);
        ApplyRows(rows, row.CommandKey);
    }

    private void MoveItem(int direction)
    {
        if (_grid.DataSource is not List<Row> rows)
        {
            return;
        }

        string? commandKey = GetSelectedCommandKey();
        if (string.IsNullOrWhiteSpace(commandKey))
        {
            return;
        }

        Row? selected = rows.FirstOrDefault(x => string.Equals(x.CommandKey, commandKey, StringComparison.OrdinalIgnoreCase));
        if (selected is null)
        {
            return;
        }

        List<Row> menuRows = rows
            .Where(x => string.Equals(x.TopLevelMenu, selected.TopLevelMenu, StringComparison.OrdinalIgnoreCase))
            .OrderBy(x => x.ItemOrder)
            .ThenBy(x => x.MenuText, StringComparer.OrdinalIgnoreCase)
            .ToList();

        int currentIndex = menuRows.FindIndex(x => string.Equals(x.CommandKey, commandKey, StringComparison.OrdinalIgnoreCase));
        int swapIndex = currentIndex + direction;

        if (currentIndex < 0 || swapIndex < 0 || swapIndex >= menuRows.Count)
        {
            return;
        }

        (menuRows[currentIndex], menuRows[swapIndex]) = (menuRows[swapIndex], menuRows[currentIndex]);

        int order = 10;
        foreach (Row row in menuRows)
        {
            row.ItemOrder = order;
            order += 10;
        }

        RenumberRows(rows);
        ApplyRows(rows, commandKey);
    }

    private void MoveMenuGroup(int direction)
    {
        if (_grid.DataSource is not List<Row> rows)
        {
            return;
        }

        string? commandKey = GetSelectedCommandKey();
        if (string.IsNullOrWhiteSpace(commandKey))
        {
            return;
        }

        Row? selected = rows.FirstOrDefault(x => string.Equals(x.CommandKey, commandKey, StringComparison.OrdinalIgnoreCase));
        if (selected is null)
        {
            return;
        }

        List<string> menuOrder = rows
            .GroupBy(x => x.TopLevelMenu)
            .OrderBy(group => group.Min(x => x.MenuOrder))
            .ThenBy(group => group.Key, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.Key)
            .ToList();

        int currentIndex = menuOrder.FindIndex(x => string.Equals(x, selected.TopLevelMenu, StringComparison.OrdinalIgnoreCase));
        int swapIndex = currentIndex + direction;

        if (currentIndex < 0 || swapIndex < 0 || swapIndex >= menuOrder.Count)
        {
            return;
        }

        (menuOrder[currentIndex], menuOrder[swapIndex]) = (menuOrder[swapIndex], menuOrder[currentIndex]);

        int menuValue = 10;
        foreach (string menuName in menuOrder)
        {
            foreach (Row row in rows.Where(x => string.Equals(x.TopLevelMenu, menuName, StringComparison.OrdinalIgnoreCase)))
            {
                row.MenuOrder = menuValue;
            }

            menuValue += 10;
        }

        RenumberRows(rows);
        ApplyRows(rows, commandKey);
    }

    private void RenumberRows(List<Row> rows)
    {
        var menuGroups = rows
            .GroupBy(x => x.TopLevelMenu)
            .OrderBy(group => group.Min(x => x.MenuOrder))
            .ThenBy(group => group.Key, StringComparer.OrdinalIgnoreCase)
            .ToList();

        int menuOrder = 10;

        foreach (var group in menuGroups)
        {
            int itemOrder = 10;

            foreach (Row row in group
                .OrderBy(x => x.ItemOrder)
                .ThenBy(x => x.MenuText, StringComparer.OrdinalIgnoreCase))
            {
                row.MenuOrder = menuOrder;
                row.ItemOrder = itemOrder;
                itemOrder += 10;
            }

            menuOrder += 10;
        }
    }

    private void ApplyRows(List<Row> rows, string? selectedCommandKey)
    {
        foreach (Row row in rows)
        {
            _registry.UpdateMetadata(row.CommandKey, row.MenuText, row.MenuOrder, row.ItemOrder);
        }

        _registry.NormalizeAndSave();
        _rebuildMenu();
        LoadGrid();
        LoadJson();
        SelectRowByCommandKey(selectedCommandKey);
    }

    private void UpdateButtons()
    {
        if (_grid.DataSource is not List<Row> rows)
        {
            _btnMenuUp.Enabled = false;
            _btnMenuDown.Enabled = false;
            _btnItemUp.Enabled = false;
            _btnItemDown.Enabled = false;
            return;
        }

        string? commandKey = GetSelectedCommandKey();
        if (string.IsNullOrWhiteSpace(commandKey))
        {
            _btnMenuUp.Enabled = false;
            _btnMenuDown.Enabled = false;
            _btnItemUp.Enabled = false;
            _btnItemDown.Enabled = false;
            return;
        }

        Row? selected = rows.FirstOrDefault(x => string.Equals(x.CommandKey, commandKey, StringComparison.OrdinalIgnoreCase));
        if (selected is null)
        {
            _btnMenuUp.Enabled = false;
            _btnMenuDown.Enabled = false;
            _btnItemUp.Enabled = false;
            _btnItemDown.Enabled = false;
            return;
        }

        var menus = rows
            .GroupBy(x => x.TopLevelMenu)
            .OrderBy(group => group.Min(x => x.MenuOrder))
            .ThenBy(group => group.Key, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.Key)
            .ToList();

        int menuIndex = menus.FindIndex(x => string.Equals(x, selected.TopLevelMenu, StringComparison.OrdinalIgnoreCase));

        var items = rows
            .Where(x => string.Equals(x.TopLevelMenu, selected.TopLevelMenu, StringComparison.OrdinalIgnoreCase))
            .OrderBy(x => x.ItemOrder)
            .ThenBy(x => x.MenuText, StringComparer.OrdinalIgnoreCase)
            .Select(x => x.CommandKey)
            .ToList();

        int itemIndex = items.FindIndex(x => string.Equals(x, selected.CommandKey, StringComparison.OrdinalIgnoreCase));

        _btnMenuUp.Enabled = menuIndex > 0;
        _btnMenuDown.Enabled = menuIndex >= 0 && menuIndex < menus.Count - 1;
        _btnItemUp.Enabled = itemIndex > 0;
        _btnItemDown.Enabled = itemIndex >= 0 && itemIndex < items.Count - 1;
    }

    private sealed class Row
    {
        public string CommandKey { get; set; } = string.Empty;
        public string TopLevelMenu { get; set; } = string.Empty;
        public string MenuText { get; set; } = string.Empty;
        public int MenuOrder { get; set; }
        public int ItemOrder { get; set; }
    }
}
