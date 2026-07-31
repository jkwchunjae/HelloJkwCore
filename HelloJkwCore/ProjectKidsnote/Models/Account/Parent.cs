using System.Text.Json;

namespace ProjectKidsnote.Models.Account;

public sealed class Parent
{
    public long Id { get; init; }
    public required string Type { get; init; }
    public required string Name { get; init; }
    public JsonElement? Picture { get; init; }
    public required string Username { get; init; }
}
