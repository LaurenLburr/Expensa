using System;

namespace CodexExpensa.App.WinForms.UI.Common;

public sealed class TagAssignmentOptions
{
    public required string EntityDisplayName { get; init; }

    public required string LoadAssignedTagsQueryName { get; init; }
    public required string SearchTagsQueryName { get; init; }
    public required string InsertTagQueryName { get; init; }
    public required string AssignTagQueryName { get; init; }
    public required string RemoveTagQueryName { get; init; }

    public string? EntityTypeValue { get; init; }

    public string EntityTypeParameterName { get; init; } = "@EntityType";
    public string EntityIdParameterName { get; init; } = "@EntityId";
    public string SearchParameterName { get; init; } = "@Search";
    public string TagIdParameterName { get; init; } = "@TagId";
    public string TagNameParameterName { get; init; } = "@TagName";
    public string AssignmentIdParameterName { get; init; } = "@AssignmentId";

    public string AssignedTagIdColumnName { get; init; } = "TagId";
    public string AssignedTagNameColumnName { get; init; } = "TagName";
    public string SearchTagIdColumnName { get; init; } = "TagId";
    public string SearchTagNameColumnName { get; init; } = "TagName";

    public string SearchPlaceholder { get; init; } = "Type to search or create a tag";
}
