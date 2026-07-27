namespace ProjectKidsnote.Models.Account;

public sealed class UserInfo
{
    public required User User { get; init; }
    public bool IsMainAdmin { get; init; }
    public List<Child> Children { get; init; } = [];
}
