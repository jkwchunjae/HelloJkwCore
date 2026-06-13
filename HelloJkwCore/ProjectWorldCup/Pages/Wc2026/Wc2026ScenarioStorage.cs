namespace ProjectWorldCup.Pages.Wc2026;

public interface IWc2026ScenarioStorage
{
    Task<Wc2026ScenarioSaveData> LoadAsync(AppUser user);
    Task SaveAsync(AppUser user, IEnumerable<Wc2026ScenarioGroup> groups);
}

public class Wc2026ScenarioStorage : IWc2026ScenarioStorage
{
    private readonly IFileSystem _fs;

    public Wc2026ScenarioStorage(IFileSystemService fsService, WorldCupOption option)
        : this(fsService.GetFileSystem(option.FileSystemSelect, option.Path))
    {
    }

    public Wc2026ScenarioStorage(IFileSystem fs)
    {
        _fs = fs;
    }

    public async Task<Wc2026ScenarioSaveData> LoadAsync(AppUser user)
    {
        if (user == null || !await _fs.FileExistsAsync(ScenarioPath(user.Id)))
        {
            return null;
        }

        return await _fs.ReadJsonAsync<Wc2026ScenarioSaveData>(ScenarioPath(user.Id));
    }

    public async Task SaveAsync(AppUser user, IEnumerable<Wc2026ScenarioGroup> groups)
    {
        if (user == null)
        {
            return;
        }

        var saveData = new Wc2026ScenarioSaveData
        {
            User = new Wc2026ScenarioSavedUser
            {
                Id = user.Id.ToString(),
                UserName = user.UserName,
                NickName = user.NickName,
            },
            UpdatedAtKst = DateTime.UtcNow.AddHours(9),
            Matches = groups
                .SelectMany(group => group.RemainingMatches)
                .Select(Wc2026ScenarioSavedMatch.Create)
                .ToList(),
        };

        await _fs.WriteJsonAsync(ScenarioPath(user.Id), saveData);
    }

    private static Func<Paths, string> ScenarioPath(StringId userId)
        => path => path[WorldCupPath.Betting2026Scenarios] + $"/{userId}.json";
}

public class Wc2026ScenarioSaveData
{
    public Wc2026ScenarioSavedUser User { get; set; }
    public DateTime UpdatedAtKst { get; set; }
    public List<Wc2026ScenarioSavedMatch> Matches { get; set; } = new();
}

public class Wc2026ScenarioSavedUser
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string NickName { get; set; }
}

public class Wc2026ScenarioSavedMatch
{
    public string Group { get; set; }
    public int? MatchNumber { get; set; }
    public string HomeTeam { get; set; }
    public string AwayTeam { get; set; }
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }

    public static Wc2026ScenarioSavedMatch Create(Wc2026ScenarioMatch match)
    {
        return new Wc2026ScenarioSavedMatch
        {
            Group = match.Group,
            MatchNumber = match.MatchNumber,
            HomeTeam = match.HomeTeam.Code,
            AwayTeam = match.AwayTeam.Code,
            HomeScore = match.HomeScore,
            AwayScore = match.AwayScore,
        };
    }
}
