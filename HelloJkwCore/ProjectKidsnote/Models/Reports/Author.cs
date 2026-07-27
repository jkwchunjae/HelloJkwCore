using System.Text.Json;

namespace ProjectKidsnote.Models.Reports;

public sealed class Author
{
    public long Id { get; init; }
    public required string Type { get; init; }
    public required string Name { get; init; }
    public JsonElement? Picture { get; init; }
    public required string Username { get; init; }
}
