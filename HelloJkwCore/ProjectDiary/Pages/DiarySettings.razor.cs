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
    private Task OnWriterSelect()
    {
        if (SearchedWriter != null)
        {
            DiaryInfo.Writers.Add(SearchedWriter.Id);
            SearchedWriter = null;
        }
        return Task.CompletedTask;
    }
    private Task OnViewerSelect()
    {
        if (SearchedViewer != null)
        {
            DiaryInfo.Viewers.Add(SearchedViewer.Id);
            SearchedViewer = null;
        }
        return Task.CompletedTask;
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
