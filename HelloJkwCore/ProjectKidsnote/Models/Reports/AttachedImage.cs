using System.Text.Json;

namespace ProjectKidsnote.Models.Reports;

public sealed class AttachedImage
{
    public JsonElement Id { get; init; }
    public required string AccessKey { get; init; }
    public required string OriginalFileName { get; init; }
    public long FileSize { get; init; }
    public int Width { get; init; }
    public int Height { get; init; }
    public required string Original { get; init; }
    public required string Large { get; init; }
    public required string Small { get; init; }
    public required string SmallResize { get; init; }
    public required string LargeResize { get; init; }
}
