using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CodexExpensa.App.WinForms.UI.Common;
using CodexExpensa.Core.Abstractions;
using Microsoft.Data.Sqlite;

namespace CodexExpensa.App.WinForms.UI.Websites;

public sealed class WebsiteDetailsForm : TaggedDetailsFormBase
{
    private readonly IDatabaseSession _db;

    private string? _websiteId;

    private readonly Label _lblName;
    private readonly TextBox _txtName;
    private readonly Label _lblUrl;
    private readonly TextBox _txtUrl;
    private readonly Label _lblNotes;
    private readonly TextBox _txtNotes;

    public WebsiteDetailsForm(
        IDatabaseSession db,
        Action onSaved)
        : base(
            titleText: "Website Details",
            onSaved: onSaved,
            db: db,
            tagOptions: new TagAssignmentOptions
            {
                EntityDisplayName = "website",
                EntityTypeValue = "Website",
                LoadAssignedTagsQueryName = "Tag.GetByEntity",
                SearchTagsQueryName = "Tag.SearchByName",
                InsertTagQueryName = "Tag.Insert",
                AssignTagQueryName = "TagAssignment.Insert",
                RemoveTagQueryName = "TagAssignment.DeleteByEntityAndTag",
                EntityTypeParameterName = "@EntityType",
                EntityIdParameterName = "@EntityId",
                SearchParameterName = "@Search",
                TagIdParameterName = "@TagId",
                TagNameParameterName = "@TagName",
                AssignmentIdParameterName = "@TagAssignmentId",
                AssignedTagIdColumnName = "TagId",
                AssignedTagNameColumnName = "TagName",
                SearchTagIdColumnName = "TagId",
                SearchTagNameColumnName = "TagName"
            })
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));

        _lblName = new Label
        {
            Text = "Name:",
            AutoSize = true,
            Parent = RootPanel
        };

        _txtName = new TextBox
        {
            Parent = RootPanel
        };

        _lblUrl = new Label
        {
            Text = "URL:",
            AutoSize = true,
            Parent = RootPanel
        };

        _txtUrl = new TextBox
        {
            Parent = RootPanel
        };

        _lblNotes = new Label
        {
            Text = "Notes:",
            AutoSize = true,
            Parent = RootPanel
        };

        _txtNotes = new TextBox
        {
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            Parent = RootPanel
        };

        TagsGroup.Parent = RootPanel;

        RootPanel.Resize += (_, _) => LayoutForm();
        LayoutForm();
    }

    private void LayoutForm()
    {
        int leftLabel = 12;
        int leftInput = 120;
        int top = TitleBottom + 16;
        int inputWidth = Math.Max(250, RootPanel.Width - leftInput - 24);

        _lblName.Left = leftLabel;
        _lblName.Top = top + 4;

        _txtName.Left = leftInput;
        _txtName.Top = top;
        _txtName.Width = inputWidth;

        top += 36;

        _lblUrl.Left = leftLabel;
        _lblUrl.Top = top + 4;

        _txtUrl.Left = leftInput;
        _txtUrl.Top = top;
        _txtUrl.Width = inputWidth;

        top += 36;

        _lblNotes.Left = leftLabel;
        _lblNotes.Top = top + 4;

        _txtNotes.Left = leftInput;
        _txtNotes.Top = top;
        _txtNotes.Width = inputWidth;
        _txtNotes.Height = 140;

        top += _txtNotes.Height + 12;

        TagsGroup.Left = 12;
        TagsGroup.Top = top;
        TagsGroup.Width = Math.Max(250, RootPanel.Width - 24);
        TagsGroup.Height = Math.Max(180, SaveButton.Top - top - 12);

        SaveButton.Left = 12;
        SaveButton.Top = RootPanel.Height - SaveButton.Height - 12;
    }

    public void LoadWebsite(string websiteId, string name, string url, string notes)
    {
        if (string.IsNullOrWhiteSpace(websiteId))
        {
            throw new ArgumentException("websiteId is required.", nameof(websiteId));
        }

        _websiteId = websiteId;
        _txtName.Text = name ?? string.Empty;
        _txtUrl.Text = url ?? string.Empty;
        _txtNotes.Text = notes ?? string.Empty;

        LoadTagsForEntity(websiteId);
    }

    protected override void OnSaveRequested()
    {
        if (string.IsNullOrWhiteSpace(_websiteId))
        {
            MessageBox.Show(this, "Load a website before saving.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        string name = _txtName.Text.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            MessageBox.Show(this, "Name is required.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _txtName.Focus();
            return;
        }

        string url = _txtUrl.Text.Trim();
        if (string.IsNullOrWhiteSpace(url))
        {
            MessageBox.Show(this, "URL is required.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _txtUrl.Focus();
            return;
        }

        IEnumerable<SqliteParameter> parameters =
        [
            new SqliteParameter("@WebsiteId", _websiteId),
            new SqliteParameter("@Name", name),
            new SqliteParameter("@Url", url),
            new SqliteParameter("@Notes", _txtNotes.Text.Trim())
        ];

        try
        {
            _db.Execute("Website.Update", parameters);
            _db.Save();
            NotifySaved();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.ToString(), "Save failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
