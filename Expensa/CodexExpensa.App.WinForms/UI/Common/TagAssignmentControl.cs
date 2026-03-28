using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CodexExpensa.Core.Abstractions;
using Microsoft.Data.Sqlite;

namespace CodexExpensa.App.WinForms.UI.Common;

public sealed class TagAssignmentControl : UserControl
{
    private readonly IDatabaseSession _db;
    private readonly TagAssignmentOptions _options;

    private string? _entityId;

    private readonly GroupBox _group;
    private readonly Label _lblSearch;
    private readonly TextBox _txtSearch;
    private readonly ListBox _lstSearch;
    private readonly Button _btnAdd;
    private readonly Label _lblAssigned;
    private readonly ListBox _lstAssigned;
    private readonly Button _btnRemove;
    private readonly Label _lblHint;

    public event EventHandler? TagsChanged;

    public TagAssignmentControl(
        IDatabaseSession db,
        TagAssignmentOptions options)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
        _options = options ?? throw new ArgumentNullException(nameof(options));

        Dock = DockStyle.Fill;

        _group = new GroupBox
        {
            Dock = DockStyle.Fill,
            Text = "Tags"
        };
        Controls.Add(_group);

        _lblSearch = new Label
        {
            Text = "Find or create tag:",
            AutoSize = true,
            Left = 12,
            Top = 28,
            Parent = _group
        };

        _txtSearch = new TextBox
        {
            Left = 12,
            Top = 48,
            Width = 260,
            Parent = _group
        };
        _txtSearch.TextChanged += (_, _) => RefreshSearchResults();

        _lstSearch = new ListBox
        {
            Left = 12,
            Top = 80,
            Width = 260,
            Height = 120,
            Parent = _group
        };
        _lstSearch.DoubleClick += (_, _) => AddSelectedOrTypedTag();

        _btnAdd = new Button
        {
            Text = "Add →",
            Left = 285,
            Top = 80,
            Width = 90,
            Height = 30,
            Parent = _group
        };
        _btnAdd.Click += (_, _) => AddSelectedOrTypedTag();

        _lblAssigned = new Label
        {
            Text = "Assigned tags:",
            AutoSize = true,
            Left = 390,
            Top = 28,
            Parent = _group
        };

        _lstAssigned = new ListBox
        {
            Left = 390,
            Top = 48,
            Width = 260,
            Height = 152,
            Parent = _group
        };
        _lstAssigned.DoubleClick += (_, _) => RemoveSelectedTag();

        _btnRemove = new Button
        {
            Text = "Remove",
            Left = 660,
            Top = 48,
            Width = 90,
            Height = 30,
            Parent = _group
        };
        _btnRemove.Click += (_, _) => RemoveSelectedTag();

        _lblHint = new Label
        {
            Left = 12,
            Top = 212,
            Width = 740,
            Height = 36,
            Text = "Type part of a tag name to search. If nothing matches, the typed text will be created as a new tag and assigned.",
            Parent = _group
        };

        Resize += (_, _) => LayoutControls();
        LayoutControls();
    }

    public void LoadForEntity(string entityId)
    {
        if (string.IsNullOrWhiteSpace(entityId))
            throw new ArgumentException("entityId is required.", nameof(entityId));

        _entityId = entityId.Trim();
        _txtSearch.Text = string.Empty;
        RefreshSearchResults();
        RefreshAssignedTags();
    }

    public void RefreshAssignedTags()
    {
        if (string.IsNullOrWhiteSpace(_entityId))
        {
            _lstAssigned.Items.Clear();
            return;
        }

        DataTable table = _db.QueryDataTable(
            _options.LoadAssignedTagsQueryName,
            new DbParameter[]
            {
                new SqliteParameter(_options.EntityIdParameterName, _entityId)
            });

        _lstAssigned.BeginUpdate();
        try
        {
            _lstAssigned.Items.Clear();

            foreach (DataRow row in table.Rows)
            {
                string tagId = Convert.ToString(row[_options.AssignedTagIdColumnName]) ?? string.Empty;
                string tagName = Convert.ToString(row[_options.AssignedTagNameColumnName]) ?? string.Empty;

                if (string.IsNullOrWhiteSpace(tagId) || string.IsNullOrWhiteSpace(tagName))
                    continue;

                if (_lstAssigned.Items.Cast<TagListItem>().Any(x => string.Equals(x.TagId, tagId, StringComparison.Ordinal)))
                    continue;

                _lstAssigned.Items.Add(new TagListItem(tagId, tagName));
            }
        }
        finally
        {
            _lstAssigned.EndUpdate();
        }
    }

    private void RefreshSearchResults()
    {
        string search = _txtSearch.Text.Trim();

        _lstSearch.BeginUpdate();
        try
        {
            _lstSearch.Items.Clear();

            HashSet<string> seenTagNames = new(StringComparer.OrdinalIgnoreCase);

            DataTable table = _db.QueryDataTable(
                _options.SearchTagsQueryName,
                new DbParameter[]
                {
                    new SqliteParameter(_options.SearchParameterName, search)
                });

            foreach (DataRow row in table.Rows)
            {
                string tagId = Convert.ToString(row[_options.SearchTagIdColumnName]) ?? string.Empty;
                string tagName = Convert.ToString(row[_options.SearchTagNameColumnName]) ?? string.Empty;

                if (string.IsNullOrWhiteSpace(tagId) || string.IsNullOrWhiteSpace(tagName))
                    continue;

                if (!seenTagNames.Add(tagName))
                    continue;

                _lstSearch.Items.Add(new TagListItem(tagId, tagName));
            }
        }
        finally
        {
            _lstSearch.EndUpdate();
        }
    }

    private void AddSelectedOrTypedTag()
    {
        if (string.IsNullOrWhiteSpace(_entityId))
        {
            MessageBox.Show(this, $"Load an {_options.EntityDisplayName} before editing tags.", "Tags", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        TagListItem? item = _lstSearch.SelectedItem as TagListItem;

        if (item is null)
        {
            string typedName = _txtSearch.Text.Trim();
            if (string.IsNullOrWhiteSpace(typedName))
                return;

            item = FindAssignedByName(typedName);
            if (item is not null)
                return;

            item = CreateOrReuseTag(typedName);
        }

        if (IsAlreadyAssigned(item.TagId) || FindAssignedByName(item.TagName) is not null)
            return;

        _db.Execute(
            _options.AssignTagQueryName,
            new DbParameter[]
            {
                new SqliteParameter(_options.AssignmentIdParameterName, Guid.NewGuid().ToString("N")),
                new SqliteParameter(_options.EntityIdParameterName, _entityId),
                new SqliteParameter(_options.TagIdParameterName, item.TagId)
            });

        _txtSearch.Text = string.Empty;
        RefreshSearchResults();
        RefreshAssignedTags();
        _db.Save();
        TagsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void RemoveSelectedTag()
    {
        if (string.IsNullOrWhiteSpace(_entityId))
            return;

        if (_lstAssigned.SelectedItem is not TagListItem item)
            return;

        _db.Execute(
            _options.RemoveTagQueryName,
            new DbParameter[]
            {
                new SqliteParameter(_options.EntityIdParameterName, _entityId),
                new SqliteParameter(_options.TagIdParameterName, item.TagId)
            });

        RefreshAssignedTags();
        _db.Save();
        TagsChanged?.Invoke(this, EventArgs.Empty);
    }

    private TagListItem? TryGetExistingTag(string tagName)
    {
        DataTable table = _db.QueryDataTable(
            _options.SearchTagsQueryName,
            new DbParameter[]
            {
                new SqliteParameter(_options.SearchParameterName, tagName)
            });

        foreach (DataRow row in table.Rows)
        {
            string existingTagId = Convert.ToString(row[_options.SearchTagIdColumnName]) ?? string.Empty;
            string existingTagName = Convert.ToString(row[_options.SearchTagNameColumnName]) ?? string.Empty;

            if (string.IsNullOrWhiteSpace(existingTagId) || string.IsNullOrWhiteSpace(existingTagName))
                continue;

            if (string.Equals(existingTagName, tagName, StringComparison.OrdinalIgnoreCase))
                return new TagListItem(existingTagId, existingTagName);
        }

        return null;
    }

    private TagListItem CreateOrReuseTag(string tagName)
    {
        TagListItem? existing = TryGetExistingTag(tagName);
        if (existing is not null)
            return existing;

        string newTagId = Guid.NewGuid().ToString("N");

        _db.Execute(
            _options.InsertTagQueryName,
            new DbParameter[]
            {
                new SqliteParameter(_options.TagIdParameterName, newTagId),
                new SqliteParameter(_options.TagNameParameterName, tagName)
            });

        return new TagListItem(newTagId, tagName);
    }

    private bool IsAlreadyAssigned(string tagId)
        => _lstAssigned.Items.Cast<TagListItem>().Any(x => string.Equals(x.TagId, tagId, StringComparison.Ordinal));

    private TagListItem? FindAssignedByName(string tagName)
        => _lstAssigned.Items.Cast<TagListItem>().FirstOrDefault(x => string.Equals(x.TagName, tagName, StringComparison.OrdinalIgnoreCase));

    private void LayoutControls()
    {
        _group.Padding = new Padding(10);

        int margin = 12;
        int centerGap = 16;
        int buttonColumnWidth = 100;
        int listHeight = Math.Max(120, Height - 110);

        int availableWidth = Math.Max(760, ClientSize.Width - 24);
        int listWidth = Math.Max(220, (availableWidth - (margin * 2) - centerGap - buttonColumnWidth - 20) / 2);

        _lblSearch.Left = margin;
        _lblSearch.Top = 28;

        _txtSearch.Left = margin;
        _txtSearch.Top = 48;
        _txtSearch.Width = listWidth;

        _lstSearch.Left = margin;
        _lstSearch.Top = 80;
        _lstSearch.Width = listWidth;
        _lstSearch.Height = listHeight;

        _btnAdd.Left = _lstSearch.Right + 12;
        _btnAdd.Top = 80;
        _btnAdd.Width = buttonColumnWidth;

        _lblAssigned.Left = _btnAdd.Right + centerGap;
        _lblAssigned.Top = 28;

        _lstAssigned.Left = _btnAdd.Right + centerGap;
        _lstAssigned.Top = 48;
        _lstAssigned.Width = listWidth;
        _lstAssigned.Height = listHeight + 32;

        _btnRemove.Left = _lstAssigned.Right + 12;
        _btnRemove.Top = 48;
        _btnRemove.Width = buttonColumnWidth;

        _lblHint.Left = margin;
        _lblHint.Top = _lstSearch.Bottom + 10;
        _lblHint.Width = Math.Max(400, ClientSize.Width - 24);
    }

    private sealed class TagListItem
    {
        public string TagId { get; }
        public string TagName { get; }

        public TagListItem(string tagId, string tagName)
        {
            TagId = tagId ?? throw new ArgumentNullException(nameof(tagId));
            TagName = tagName ?? throw new ArgumentNullException(nameof(tagName));
        }

        public override string ToString() => TagName;
    }
}
