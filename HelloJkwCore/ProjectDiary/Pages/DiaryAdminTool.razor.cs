using Microsoft.AspNetCore.Identity;

namespace ProjectDiary.Pages;

public partial class DiaryAdminTool : JkwPageBase
{
    [Inject] IDiaryService DiaryService { get; set; }
    [Inject] IDiaryAdminService DiaryAdminService { get; set; }
    [Inject] IDiarySearchService DiarySearchService { get; set; }
    [Inject] private UserManager<AppUser> UserManager { get; set; }

    IEnumerable<DiaryData> DiaryDataList { get; set; } = new List<DiaryData>();

    Dictionary<string, (bool ProgressOn, int ProgressTotal, int ProgressValue)> ProgressDic = new();

    protected override async Task OnPageInitializedAsync()
    {
        if (IsAuthenticated && User.HasRole(UserRole.Admin))
        {
            var list = await DiaryAdminService.GetAllDiaryListAsync(User);

            DiaryDataList = await list
                .Select(async x => await CreateDiaryData(x))
                .WhenAll();
        }
    }

    private async Task<DiaryData> CreateDiaryData(DiaryInfo diaryInfo)
    {
        var diaryData = new DiaryData(diaryInfo);

        var files = await DiaryService.GetDiaryFileAllAsync(User, diaryInfo);
        diaryData.DiaryFileList = files;

        var user = await UserManager.FindByIdAsync(diaryInfo.Owner.Id);
        diaryData.OwnerUser = user;

        return diaryData;
    }

    async Task CreateTrie(DiaryData diaryData)
    {
        if (diaryData.IsSecret)
        {
            return;
        }

        diaryData.Progress = (true, diaryData.DiaryFileList.Count, 0);
        StateHasChanged();

        await DiarySearchService.ClearTrie(diaryData.DiaryName);

        var progressValue = 0;
        foreach (var fileName in diaryData.DiaryFileList)
        {
            progressValue++;
            var content = await DiaryService.GetDiaryContentAsync(User, diaryData, fileName);
            await DiarySearchService.AppendDiaryTextAsync(diaryData.DiaryName, fileName, content.Text);

            if (diaryData.Progress.Total > 100)
            {
                if (progressValue % 10 == 0)
                {
                    diaryData.Progress = (true, diaryData.DiaryFileList.Count, progressValue);
                    StateHasChanged();
                }
            }
            else
            {
                diaryData.Progress = (true, diaryData.DiaryFileList.Count, progressValue);
                StateHasChanged();
            }
        }
        diaryData.Progress = (true, diaryData.DiaryFileList.Count, diaryData.DiaryFileList.Count);
        StateHasChanged();

        var years = diaryData.DiaryFileList
            .Select(diaryFile => diaryFile.Date.Year)
            .Distinct()
            .OrderBy(year => year)
            .ToArray();

        await years
            .Select(year => DiarySearchService.SaveDiaryTrie(diaryData.DiaryName, year))
            .WhenAll();
    }

    private async Task CreateTrieYear(DiaryData diaryData, int year)
    {
        if (diaryData.IsSecret)
        {
            return;
        }

        await DiarySearchService.ClearTrieYear(diaryData.DiaryName, year);

        var diaryFileList = diaryData.DiaryFileList
            .Where(filename => filename.Date.Year == year)
            .ToArray();
        var progressTotal = diaryFileList.Length + 1;

        diaryData.Progress = (true, progressTotal, 0);
        StateHasChanged();

        var progressValue = 0;
        foreach (var filename in diaryFileList)
        {
            var content = await DiaryService.GetDiaryContentAsync(User, diaryData, filename);
            await DiarySearchService.AppendDiaryTextAsync(diaryData.DiaryName, filename, content.Text);
            progressValue++;

            diaryData.Progress = (true, progressTotal, progressValue);
            StateHasChanged();
        }

        await DiarySearchService.SaveDiaryTrie(diaryData.DiaryName, year);

        progressValue++;
        diaryData.Progress = (true, progressTotal, progressValue);
        StateHasChanged();
    }
}

class DiaryData : DiaryInfo
{
    public List<DiaryFileName> DiaryFileList { get; set; }
    public (bool On, int Total, int Value) Progress { get; set; }
    public AppUser OwnerUser { get; set; }
    public int SelectedYear { get; set; }
    public string OwnerName
    {
        get
        {
            if (OwnerUser == null)
            {
                return Owner.Id;
            }
            else if (OwnerUser?.NickName != null)
            {
                if (OwnerUser.NickName != OwnerUser.UserName)
                {
                    return $"{OwnerUser.NickName} ({OwnerUser.UserName})";
                }
            }

            return OwnerUser.DisplayName;
        }
    }

    public bool Disabled_CreateTrie => Progress.On && (Progress.Total != Progress.Value);

    public DiaryData(DiaryInfo info)
        :base(info)
    {
        Progress = (false, 0, 0);
    }
}