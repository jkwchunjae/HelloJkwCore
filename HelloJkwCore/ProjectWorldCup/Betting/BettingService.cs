namespace ProjectWorldCup;

public partial class BettingService : IBettingService
{
    private readonly IFileSystem _fs;

    public BettingService(
        IFileSystemService fsService,
        WorldCupOption option)
    {
        _fs = fsService.GetFileSystem(option.FileSystemSelect, option.Path);
        _fs2018 = fsService.GetFileSystem(option.FileSystemSelect2018, option.Path);
    }

    public Task<BettingUser> JoinBettingAsync(BettingUser user, BettingType bettingType)
    {
        throw new NotImplementedException();
    }

    private Task SaveBettingItemAsync(BettingType bettingType, IWcBettingItem<Team> item)
    {
        return Task.CompletedTask;
    }
}
