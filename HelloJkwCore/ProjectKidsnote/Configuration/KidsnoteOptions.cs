using Common;

namespace ProjectKidsnote.Configuration;

public sealed class KidsnoteOptions
{
    public const string SectionName = "Kidsnote";

    public string BaseAddress { get; set; } = "https://www.kidsnote.com/api/";
    public string TimeZoneId { get; set; } = "Asia/Seoul";
    public FileSystemSelectOption FileSystemSelect { get; set; } = new();
    public PathMap Path { get; set; } = new();

}
