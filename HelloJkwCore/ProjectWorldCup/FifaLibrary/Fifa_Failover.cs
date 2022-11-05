namespace ProjectWorldCup.FifaLibrary;

public partial class Fifa : IFifa
{
    public async Task<IEnumerable<string>> GetFailoverList()
    {
        var defaultList = new[]
        {
            "GroupStageMatches.json",
            "Round16Matches.json",
            "AfterRound16Matches.json",
            "GroupOverview.json",
            "StandingData.json",
        };
        var list = await _fs.GetFilesAsync(path => path["FifaData"], ".json");
        return defaultList.Concat(list).Distinct();
    }
    public async Task<string> GetFailoverData(string filename)
    {
        if (await _fs.FileExistsAsync(path => path["FifaData"] + $"/{filename}"))
        {
            return await _fs.ReadTextAsync(path => path["FifaData"] + $"/{filename}");
        }
        else
        {
            return string.Empty;
        }
    }
    public async Task SaveFailoverData(string title, string value)
    {
        await _fs.WriteTextAsync(path => path["FifaData"] + $"/{title}", value);
    }
    private async Task<T> GetFailoverData<T>(string filename)
    {
        try
        {
            return await _fs.ReadJsonAsync<T>(path => path["FifaData"] + $"/{filename}");
        }
        catch
        {
            return default;
        }
    }
    public async Task SaveFailoverData<T>(string title, T value)
    {
        await _fs.WriteJsonAsync<T>(path => path["FifaData"] + $"/{title}", value);
    }
}
