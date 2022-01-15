namespace ProjectSuFc;

public partial class SuFcService : ISuFcService
{
    IFileSystem _fs;

    public SuFcService(
        SuFcOption option,
        IFileSystemService fsService)
    {
        _fs = fsService.GetFileSystem(option.FileSystemSelect, option.Path);

        InitTeamMaker();
    }
}