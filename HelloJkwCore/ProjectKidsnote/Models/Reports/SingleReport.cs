using System.Text.Json;

namespace ProjectKidsnote.Models.Reports;

public sealed class SingleReport
{
    public long Id { get; init; }
    public DateTimeOffset Created { get; init; }
    public DateTimeOffset Modified { get; init; }
    public required string DateWritten { get; init; }
    public required Author Author { get; init; }
    public required string AuthorName { get; init; }
    public long Center { get; init; }
    public long Cls { get; init; }
    public required string ClassName { get; init; }
    public long Child { get; init; }
    public required string ChildName { get; init; }
    public JsonElement? ChildPicture { get; init; }
    public bool IsSentFromCenter { get; init; }
    public required string Content { get; init; }
    public string? Weather { get; init; }
    public JsonElement? AttachedVideo { get; init; }
    public int NumComments { get; init; }
    public bool ReadByMe { get; init; }
    public ReadByParent? ReadByParent { get; init; }
    public int ReactionCount { get; init; }
    public JsonElement? ActivityRate { get; init; }
    public List<AttachedImage> AttachedImages { get; init; } = [];
    public List<JsonElement> AttachedFiles { get; init; } = [];
    public string? Thumbnail { get; init; }
}
