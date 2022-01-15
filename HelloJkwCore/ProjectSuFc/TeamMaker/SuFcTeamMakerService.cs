namespace ProjectSuFc;

public partial class SuFcService : ISuFcService
{
    private readonly Dictionary<TeamMakerStrategy, ITeamMaker> _strategy = new();
    private List<TeamResult> _teamResultList = null;

    private void InitTeamMaker()
    {
        _strategy.Add(TeamMakerStrategy.FullRandom, new FullRandomTeamMaker());
        _strategy.Add(TeamMakerStrategy.Manual, new ManualTeamMaker());
        //_strategy.Add(TeamMakerStrategy.AttendProb, new AttendProbTeamMaker());
        _strategy.Add(TeamMakerStrategy.TeamSetting, new SettingOptionTeamMaker());
    }

    public async Task<TeamResult> MakeTeam(List<MemberName> members, int teamCount, TeamMakerStrategy strategy, TeamSettingOption option)
    {
        _teamResultList = null;

        if (_strategy.ContainsKey(strategy))
        {
            var teamMaker = _strategy[strategy];
            var result = await teamMaker.MakeTeamAsync(members, teamCount, option);
            return result;
        }

        return null;
    }

    public async Task<List<TeamResult>> GetAllTeamResult()
    {
        var cache = _teamResultList;
        if (cache != null)
            return cache;

        var files = await _fs.GetFilesAsync(path => path[SuFcPathType.SuFcTeamsPath]);

        var list = await files.Select(async file =>
            {
                return await _fs.ReadJsonAsync<TeamResult>(path => path[SuFcPathType.SuFcTeamsPath] + "/" + file);
            })
            .WhenAll();

        cache = list.ToList();
        _teamResultList = cache;
        return cache;
    }

    public async Task<TeamResult> FindTeamResult(string title)
    {
        var found = _teamResultList?.Find(x => x.Title == title);
        if (found != null)
            return found;

        var fileName = $"{title}.json";
        var saveFile = await _fs.ReadJsonAsync<TeamResult>(path => path[SuFcPathType.SuFcTeamsPath] + "/" + fileName);
        return saveFile;
    }

    public async Task<bool> SaveTeamResult(TeamResult saveFile)
    {
        var fileName = $"{saveFile.Title}.json";

        if (fileName.HasInvalidFileNameChar())
        {
            return false;
        }

        await _fs.WriteJsonAsync(path => path[SuFcPathType.SuFcTeamsPath] + "/" + fileName, saveFile);

        _teamResultList = null;

        return true;
    }

    public async Task<TeamSettingOption> GetTeamSettingOption()
    {
        if (await _fs.FileExistsAsync(path => path[SuFcPathType.SuFcTeamSettingFile]))
        {
            var option = await _fs.ReadJsonAsync<TeamSettingOption>(path => path[SuFcPathType.SuFcTeamSettingFile]);
            return option ?? new();
        }
        return new();
    }

    public async Task SaveTeamSettingOption(TeamSettingOption option)
    {
        await _fs.WriteJsonAsync(path => path[SuFcPathType.SuFcTeamSettingFile], option);
    }
}