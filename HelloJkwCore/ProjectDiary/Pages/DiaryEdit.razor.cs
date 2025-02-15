﻿
namespace ProjectDiary.Pages;

public partial class DiaryEdit : JkwPageBase, IDisposable
{
    [Inject] public IDiaryService DiaryService { get; set; }
    [Inject] private UserInstantData UserData { get; set; }

    [Parameter] public string DiaryName { get; set; }
    [Parameter] public string Date { get; set; }

    private DiaryInfo DiaryInfo { get; set; }
    private DiaryView View { get; set; }

    private bool HasDiaryInfo => DiaryInfo != null;
    private bool HasDiaryContent => HasDiaryInfo && (View?.DiaryContents?.Any() ?? false);
    private Dictionary<int, int> Rows => View?.DiaryContents?
        .ToDictionary(x => x.Index, x => x.Text.Split('\n').Count())
        ?? new();

    private bool ContentHasError { get; set; }
    private bool HasError { get; set; }

    public void Dispose()
    {
        Navi.LocationChanged -= HandleLocationChanged;
    }

    protected override async Task OnPageInitializedAsync()
    {
        Navi.LocationChanged += HandleLocationChanged;
        if (!IsAuthenticated)
        {
            Navi.NavigateTo("/login");
            return;
        }

        await InitDiary();

        StateHasChanged();
    }

    private async void HandleLocationChanged(object sender, LocationChangedEventArgs e)
    {
        try
        {
            await HandleLocationChanged(e);
        }
        catch
        {
        }
    }

    protected async Task HandleLocationChanged(LocationChangedEventArgs e)
    {
        await InitDiary();

        StateHasChanged();
    }

    private async Task InitDiary()
    {
        DiaryInfo = null;
        View = null;

        if (!string.IsNullOrWhiteSpace(DiaryName))
        {
            DiaryInfo = await DiaryService.GetDiaryInfoAsync(User, new DiaryName(DiaryName));
        }
        else
        {
            var userDiaryInfo = await DiaryService.GetUserDiaryInfoAsync(User);
            if (userDiaryInfo.MyDiaryList.Empty())
                return;

            var diaryName = userDiaryInfo.MyDiaryList.First();
            DiaryInfo = await DiaryService.GetDiaryInfoAsync(User, diaryName);
        }

        if (DiaryInfo == null)
            return;

        if (DiaryInfo.IsSecret && string.IsNullOrWhiteSpace(UserData.Password))
        {
            Navi.NavigateTo(DiaryUrl.SetPassword());
            return;
        }
        //WritableDiary = await DiaryService.GetWritableDiaryInfoAsync(User);
        //ViewableDiary = await DiaryService.GetViewableDiaryInfoAsync(User);

        if (HasDiaryInfo)
        {
            if (string.IsNullOrWhiteSpace(Date))
            {
                View = await DiaryService.GetLastDiaryViewAsync(User, DiaryInfo);
            }
            else if (Date.TryToDate(out var parsedDate))
            {
                View = await DiaryService.GetDiaryViewAsync(User, DiaryInfo, parsedDate);
            }

            if (DiaryInfo.IsSecret && HasDiaryContent)
            {
                foreach (var content in View?.DiaryContents)
                {
                    try
                    {
                        content.Text = content.Text.Decrypt(UserData.Password);
                    }
                    catch
                    {
                        content.Text = "임호화된 일기를 해석하지 못했습니다. 비밀번호를 다시 확인해주세요.";
                    }
                }
            }
        }
    }

    private async Task EditDiary()
    {
        if (!IsAuthenticated)
            return;

        if (DiaryInfo == null)
            return;

        if (ContentHasError)
            return;

        try
        {
            List<DiaryContent> content;
            if (DiaryInfo.IsSecret)
            {
                content = await DiaryService.UpdateDiaryAsync(User, DiaryInfo, View.DiaryContents, UserData.Password);
            }
            else
            {
                content = await DiaryService.UpdateDiaryAsync(User, DiaryInfo, View.DiaryContents);
            }

            if (content?.Any() ?? false)
            {
                Navi.NavigateTo(DiaryUrl.DiaryContent(DiaryInfo.DiaryName, content.First().Date));
            }
            else
            {
                Navi.NavigateTo(DiaryUrl.Home(DiaryInfo.DiaryName));
            }
        }
        catch
        {
            HasError = true;
        }
    }

    void OnContentErrorStateChanged(bool error)
    {
        ContentHasError = error;
    }
}