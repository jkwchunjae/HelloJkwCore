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
}
