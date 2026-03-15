using System.Data.Common;
using System.Text.Json;
using CodexExpensa.Core.Abstractions;
using Microsoft.Data.Sqlite;

namespace CodexExpensa.App.WinForms.Services;

public sealed class GridLayoutService
{
    private readonly IDatabaseSession _db;

    public GridLayoutService(IDatabaseSession db)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public void LoadLayout(
        DataGridView grid,
        string settingName,
        string? forcedFirstColumn = null,
        bool freezeForcedFirstColumn = false)
    {
        if (grid is null)
            throw new ArgumentNullException(nameof(grid));

        if (string.IsNullOrWhiteSpace(settingName))
            throw new ArgumentException("Setting name is required.", nameof(settingName));

        string? json = GetSetting(settingName);
        if (string.IsNullOrWhiteSpace(json))
        {
            ApplyForcedFirstColumn(grid, forcedFirstColumn, freezeForcedFirstColumn);
            return;
        }

        GridLayoutDto? layout;
        try
        {
            layout = JsonSerializer.Deserialize<GridLayoutDto>(json);
        }
        catch
        {
            ApplyForcedFirstColumn(grid, forcedFirstColumn, freezeForcedFirstColumn);
            return;
        }

        if (layout is null || layout.Columns.Count == 0)
        {
            ApplyForcedFirstColumn(grid, forcedFirstColumn, freezeForcedFirstColumn);
            return;
        }

        foreach (GridColumnLayoutDto saved in layout.Columns.OrderBy(c => c.DisplayIndex))
        {
            if (!grid.Columns.Contains(saved.Name))
                continue;

            DataGridViewColumn column = grid.Columns[saved.Name];

            if (saved.Width > 0)
                column.Width = saved.Width;

            if (saved.DisplayIndex >= 0 && saved.DisplayIndex < grid.Columns.Count)
                column.DisplayIndex = saved.DisplayIndex;

            column.Visible = saved.Visible;
        }

        ApplyForcedFirstColumn(grid, forcedFirstColumn, freezeForcedFirstColumn);
    }

    public void SaveLayout(DataGridView grid, string settingName)
    {
        if (grid is null)
            throw new ArgumentNullException(nameof(grid));

        if (string.IsNullOrWhiteSpace(settingName))
            throw new ArgumentException("Setting name is required.", nameof(settingName));

        GridLayoutDto layout = new()
        {
            Columns = grid.Columns
                .Cast<DataGridViewColumn>()
                .Select(c => new GridColumnLayoutDto
                {
                    Name = c.Name,
                    DisplayIndex = c.DisplayIndex,
                    Width = c.Width,
                    Visible = c.Visible
                })
                .OrderBy(c => c.DisplayIndex)
                .ToList()
        };

        string json = JsonSerializer.Serialize(layout);
        UpsertSetting(settingName, json);
    }

    private void ApplyForcedFirstColumn(
        DataGridView grid,
        string? forcedFirstColumn,
        bool freezeForcedFirstColumn)
    {
        if (string.IsNullOrWhiteSpace(forcedFirstColumn))
            return;

        if (!grid.Columns.Contains(forcedFirstColumn))
            return;

        DataGridViewColumn column = grid.Columns[forcedFirstColumn];
        column.DisplayIndex = 0;
        column.Frozen = freezeForcedFirstColumn;
    }

    private string? GetSetting(string settingName)
    {
        var table = _db.QueryDataTable(
            "UserSettings.SelectByName",
            new DbParameter[]
            {
                new SqliteParameter("@SettingName", settingName)
            });

        if (table.Rows.Count == 0)
            return null;

        return table.Rows[0]["SettingValue"]?.ToString();
    }

    private void UpsertSetting(string settingName, string settingValue)
    {
        _db.Execute(
            "UserSettings.Upsert",
            new DbParameter[]
            {
                new SqliteParameter("@SettingName", settingName),
                new SqliteParameter("@SettingValue", settingValue)
            });
    }

    private sealed class GridLayoutDto
    {
        public List<GridColumnLayoutDto> Columns { get; set; } = new();
    }

    private sealed class GridColumnLayoutDto
    {
        public string Name { get; set; } = string.Empty;

        public int DisplayIndex { get; set; }

        public int Width { get; set; }

        public bool Visible { get; set; } = true;
    }
}