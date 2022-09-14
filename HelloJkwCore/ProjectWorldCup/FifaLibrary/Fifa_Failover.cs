namespace ProjectWorldCup.FifaLibrary;

public partial class Fifa : IFifa
{
    public async Task<IEnumerable<string>> GetFailoverList()
    {
        return await _fs.GetFilesAsync(path => path["FifaData"], ".json");
    }
    public async Task<string> GetFailoverData(string filename)
    {
        return await _fs.ReadTextAsync(path => path["FifaData"] + $"/{filename}");
    }
    public async Task SaveFailoverData(string title, string value)
    {
        await _fs.WriteTextAsync(path => path["FifaData"] + $"/{title}", value);
    }
}
