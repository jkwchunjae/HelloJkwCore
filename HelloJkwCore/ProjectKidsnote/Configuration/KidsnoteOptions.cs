namespace ProjectKidsnote.Configuration;

public sealed class KidsnoteOptions
{
    public const string SectionName = "Kidsnote";

    public string BaseAddress { get; set; } = "https://www.kidsnote.com/api/";
    public string TimeZoneId { get; set; } = "Asia/Seoul";
    public string? UserId { get; set; }
    public string? Password { get; set; }

    public bool HasCredentials =>
        !string.IsNullOrWhiteSpace(UserId) &&
        !string.IsNullOrWhiteSpace(Password);
}
