using System;
using System.Windows.Forms;
using CodexExpensa.Core.Abstractions;

namespace CodexExpensa.App.WinForms.UI.Common;

public abstract class TaggedDetailsFormBase : DetailsFormBase
{
    private readonly IDatabaseSession _db;

    protected GroupBox TagsGroup { get; }
    protected TagAssignmentControl TagAssignmentControl { get; }

    protected TaggedDetailsFormBase(
        string titleText,
        Action onSaved,
        IDatabaseSession db,
        TagAssignmentOptions tagOptions)
        : base(titleText, onSaved)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));

        if (tagOptions is null)
            throw new ArgumentNullException(nameof(tagOptions));

        TagsGroup = new GroupBox
        {
            Text = "Tags"
        };

        TagAssignmentControl = new TagAssignmentControl(_db, tagOptions)
        {
            Parent = TagsGroup,
            Dock = DockStyle.Fill
        };
        TagAssignmentControl.TagsChanged += (_, _) => OnTagsChanged();
    }

    protected void LoadTagsForEntity(string entityId)
    {
        TagAssignmentControl.LoadForEntity(entityId);
    }

    protected virtual void OnTagsChanged()
    {
        _db.Save();
        NotifySaved();
    }
}
