using Microsoft.AspNetCore.Identity;
using MudBlazor;

namespace ProjectDiary.Pages;

public partial class DiarySettings : JkwPageBase
{
    [Parameter] public string DiaryName { get; set; }

    [Inject] public ISnackbar Snackbar { get; set; }
    [Inject] public IDiaryService DiaryService { get; set; }
    [Inject] public UserManager<AppUser> UserManager { get; set; }

    private DiaryInfo DiaryInfo;

    protected override async Task OnPageInitializedAsync()
    {
        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;

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
        {
            Snackbar.Add("일기장 주인은 추가할 수 없습니다.", Severity.Warning);
            return;
        }

        await DiaryService.UpdateUserDiaryInfoAsync(writer, userDiaryInfo =>
        {
            if (userDiaryInfo.WriterList?.Contains(DiaryInfo.DiaryName) ?? false)
                return false;

            userDiaryInfo.WriterList ??= new();
            userDiaryInfo.WriterList.Add(DiaryInfo.DiaryName);

            if (userDiaryInfo.ViewList?.Contains(DiaryInfo.DiaryName) ?? false)
            {
                userDiaryInfo.ViewList.Remove(DiaryInfo.DiaryName);
            }
            return true;
        });

        var result = await DiaryService.UpdateDiaryInfoAsync(User, DiaryInfo.DiaryName, diaryInfo =>
        {
            if (diaryInfo.Writers?.Contains(writer.Id) ?? false)
                return false;

            diaryInfo.Writers ??= new();
            diaryInfo.Writers.Add(writer.Id);

            if (diaryInfo.Viewers?.Contains(writer.Id) ?? false)
            {
                diaryInfo.Viewers.Remove(writer.Id);
            }
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
        {
            Snackbar.Add("일기장 주인은 추가할 수 없습니다.");
            return;
        }
        if (DiaryInfo.Writers?.Contains(viewer.Id) ?? false)
        {
            Snackbar.Add("일기장에 글을 쓸 수 있는 사람은 추가할 수 없습니다.");
            return;
        }

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

    private async Task OnDeleteWriter(AppUser writer)
    {
        var diaryName = DiaryInfo.DiaryName;
        var result1 = await DiaryService.UpdateUserDiaryInfoAsync(writer, userDiaryInfo =>
        {
            if (userDiaryInfo.WriterList?.Contains(diaryName) ?? false)
            {
                userDiaryInfo.WriterList.Remove(diaryName);
                return true;
            }
            return false;
        });

        if (!result1.Success)
            return;

        var result2 = await DiaryService.UpdateDiaryInfoAsync(User, DiaryInfo.DiaryName, diaryInfo =>
        {
            diaryInfo.Writers.RemoveAll(id => id == writer?.Id);
            return true;
        });

        if (result2.Success)
        {
            DiaryInfo = result2.Result;
        }
    }

    private async Task OnDeleteViewer(AppUser viewer)
    {
        var diaryName = DiaryInfo.DiaryName;
        var result1 = await DiaryService.UpdateUserDiaryInfoAsync(viewer, userDiaryInfo =>
        {
            if (userDiaryInfo.ViewList?.Contains(diaryName) ?? false)
            {
                userDiaryInfo.ViewList.Remove(diaryName);
                return true;
            }
            return false;
        });

        if (!result1.Success)
            return;

        var result2 = await DiaryService.UpdateDiaryInfoAsync(User, DiaryInfo.DiaryName, diaryInfo =>
        {
            diaryInfo.Viewers.RemoveAll(id => id == viewer?.Id);
            return true;
        });

        if (result2.Success)
        {
            DiaryInfo = result2.Result;
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
