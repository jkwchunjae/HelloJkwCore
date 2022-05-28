using Microsoft.AspNetCore.Identity;

namespace ProjectDiary.Pages;

public partial class DiarySettings : JkwPageBase
{
    [Parameter] public string DiaryName { get; set; }

    [Inject] public IDiaryService DiaryService { get; set; }
    [Inject] public UserManager<AppUser> UserManager { get; set; }

    private DiaryInfo DiaryInfo;

    protected override async Task OnPageInitializedAsync()
    {
        if (IsAuthenticated)
        {
            DiaryInfo = await GetDiaryInfoAsync(new DiaryName(DiaryName));
        }
    }

    private async Task<DiaryInfo> GetDiaryInfoAsync(DiaryName diaryName)
    {
        var diaryInfo = await DiaryService.GetDiaryInfoAsync(User, diaryName);
        return diaryInfo;
    }

    private async Task OnWriterSelect()
    {
        if (SearchedWriter == null)
            return;

        var writer = SearchedWriter;
        SearchedWriter = null;

        if (DiaryInfo.Owner == writer.Id)
            return;

        await DiaryService.UpdateUserDiaryInfoAsync(writer, userDiaryInfo =>
        {
            if (userDiaryInfo.WriterList?.Contains(DiaryInfo.DiaryName) ?? false)
                return false;

            userDiaryInfo.WriterList ??= new();
            userDiaryInfo.WriterList.Add(DiaryInfo.DiaryName);
            return true;
        });

        var result = await DiaryService.UpdateDiaryInfoAsync(User, DiaryInfo.DiaryName, diaryInfo =>
        {
            if (diaryInfo.Writers?.Contains(writer.Id) ?? false)
                return false;

            diaryInfo.Writers ??= new();
            diaryInfo.Writers.Add(writer.Id);
            return true;
        });

        if (result.Success)
        {
            DiaryInfo = result.Result;
        }
    }

    private async Task OnViewerSelect()
    {
        if (SearchedViewer == null)
            return;

        var viewer = SearchedViewer;
        SearchedViewer = null;

        if (DiaryInfo.Owner == viewer.Id)
            return;

        await DiaryService.UpdateUserDiaryInfoAsync(viewer, userDiaryInfo =>
        {
            if (userDiaryInfo.ViewList?.Contains(DiaryInfo.DiaryName) ?? false)
                return false;

            userDiaryInfo.ViewList ??= new();
            userDiaryInfo.ViewList.Add(DiaryInfo.DiaryName);
            return true;
        });

        var result = await DiaryService.UpdateDiaryInfoAsync(User, DiaryInfo.DiaryName, diaryInfo =>
        {
            if (diaryInfo.Viewers?.Contains(viewer.Id) ?? false)
                return false;

            diaryInfo.Viewers ??= new();
            diaryInfo.Viewers.Add(viewer.Id);
            return true;
        });

        if (result.Success)
        {
            DiaryInfo = result.Result;
        }
    }

    #region Search User

    private AppUser SearchedWriter;
    private AppUser SearchedViewer;
    private IList<AppUser> _allUsers;
    private async Task<IEnumerable<AppUser>> SearchUser(string keyword)
    {
        if (!IsAuthenticated)
            return new List<AppUser>();

        if (_allUsers == null)
        {
            _allUsers = await UserManager.GetUsersInRoleAsync("all");
        }

        var filtered = _allUsers
            .Where(user => user != User)
            .Where(user => user.DisplayName.Contains(keyword, StringComparison.InvariantCultureIgnoreCase)
                        || (user.Email?.Contains(keyword, StringComparison.InvariantCultureIgnoreCase) ?? true))
            .Take(3)
            .ToList();

        return filtered;
    }
    #endregion
}
