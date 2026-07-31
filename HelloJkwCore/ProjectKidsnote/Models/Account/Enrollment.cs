using System.Text.Json;

namespace ProjectKidsnote.Models.Account;

public sealed class Enrollment
{
    public long Id { get; init; }
    public DateTimeOffset Created { get; init; }
    public DateTimeOffset Modified { get; init; }
    public long Child { get; init; }
    public long ChildId { get; init; }
    public required string ChildName { get; init; }
    public required string ChildBirth { get; init; }
    public JsonElement? ChildPicture { get; init; }
    public required string ParentName { get; init; }
    public long CenterId { get; init; }
    public required string CenterName { get; init; }
    public long BelongToClass { get; init; }
    public required string ClassName { get; init; }
    public bool IsApproved { get; init; }
    public bool RemovedChild { get; init; }
    public bool IsExtraParent { get; init; }
}
