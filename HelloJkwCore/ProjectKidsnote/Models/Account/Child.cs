using System.Text.Json;

namespace ProjectKidsnote.Models.Account;

public sealed class Child
{
    public long Id { get; init; }
    public DateTimeOffset Created { get; init; }
    public required string Name { get; init; }
    public required string DateBirth { get; init; }
    public required string Gender { get; init; }
    public JsonElement? Picture { get; init; }
    public required Parent Parent { get; init; }
    public List<Enrollment> Enrollment { get; init; } = [];
    public required string FamilyType { get; init; }
}
