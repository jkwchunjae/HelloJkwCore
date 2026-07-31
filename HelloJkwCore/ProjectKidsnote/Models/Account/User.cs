using System.Text.Json;

namespace ProjectKidsnote.Models.Account;

public sealed class User
{
    public long Id { get; init; }
    public required string Username { get; init; }
    public required string Type { get; init; }
    public required string Name { get; init; }
    public required string CountryCode { get; init; }
    public required string Phone { get; init; }
    public required string Email { get; init; }
    public required string Description { get; init; }
    public JsonElement? Picture { get; init; }
    public DateTimeOffset DateJoined { get; init; }
    public bool Subscription { get; init; }
    public DateTimeOffset? SubscriptionUpdatedAt { get; init; }
    public bool UseMarketing { get; init; }
    public bool ThirdPartyConsent { get; init; }
    public string? DateStoreAllowed { get; init; }
    public bool IsStaff { get; init; }
    public bool ShowChangePassword { get; init; }
    public bool UsePrivacy { get; init; }
    public bool UseSecondFactor { get; init; }
}
