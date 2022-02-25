namespace ProjectDiary.Pages;

public partial class DiaryWrite : JkwPageBase
{
    [Inject] private IDiaryService DiaryService { get; set; }
    [Inject] private IDiaryTemporaryService DiaryTemporaryService { get; set; }
    [Inject] private UserInstantData UserData { get; set; }

    [Parameter]
    public string DiaryName { get; set; }
    [Parameter]
    public string DiaryDate { get; set; }

    private DiaryInfo DiaryInfo { get; set; }
    private DateTime? Date { get; set; }
    private string Content { get; set; }
    private bool ContentHasError { get; set; }
    private bool HasError { get; set; }

    protected override async Task OnPageInitializedAsync()
    {
        if (!IsAuthenticated)
        {
            Navi.NavigateTo("/login");
            return;
        }

        var list = await DiaryService.GetWritableDiaryInfoAsync(User);
        DiaryInfo = list.FirstOrDefault(x => x.DiaryName == DiaryName);

        if (DiaryInfo == null)
        {
            Navi.NavigateTo(DiaryUrl.Home(new DiaryName(DiaryName)));
        }

        if (DiaryInfo.IsSecret && string.IsNullOrWhiteSpace(UserData.Password))
        {
            Navi.NavigateTo(DiaryUrl.SetPassword());
            return;
        }

        Date = DateTime.Today;

        if (!string.IsNullOrWhiteSpace(DiaryDate))
        {
            if (DiaryDate.TryToDate(out var parsedDate))
            {
                Date = parsedDate;
            }
        }

        var tempData = await TryGetTemporaryAsync();

        if (tempData.Found)
        {
            Date = tempData.Date;
            Content = tempData.Content;
        }
    }

    async Task WriteDiaryAsync()
    {
        if (!IsAuthenticated)
            return;

        if (DiaryInfo == null)
            return;

        if (ContentHasError)
            return;

        try
        {
            DiaryContent content;
            if (DiaryInfo.IsSecret)
            {
                content = await DiaryService.WriteDiaryAsync(User, DiaryInfo, Date.Value, Content, UserData.Password);
            }
            else
            {
                content = await DiaryService.WriteDiaryAsync(User, DiaryInfo, Date.Value, Content);
            }

            await DiaryTemporaryService.RemoveTemporaryDiary(User, DiaryInfo);

            if (content != null)
            {
                Navi.NavigateTo(DiaryUrl.DiaryContent(DiaryInfo.DiaryName, Date.Value));
            }
        }
        catch
        {
            HasError = true;
        }
    }

    async Task OnContentChanged(string content)
    {
        Content = content;

        if (string.IsNullOrEmpty(Content))
        {
            await DiaryTemporaryService.RemoveTemporaryDiary(User, DiaryInfo);
        }
        else
        {
            await SaveTemporaryAsync();
        }
    }

    async Task OnDateChanged(DateTime? date)
    {
        Date = date;
        await SaveTemporaryAsync();
    }

    void OnContentErrorStateChanged(bool error)
    {
        ContentHasError = error;
    }

    async Task SaveTemporaryAsync()
    {
        if (!IsAuthenticated)
            return;
        if (DiaryInfo == null)
            return;

        await DiaryTemporaryService.SaveTemporaryDiary(User, DiaryInfo, Date.Value, Content);
    }

    async Task<(bool Found, DateTime Date, string Content)> TryGetTemporaryAsync()
    {
        if (!IsAuthenticated)
            return (false, DateTime.MinValue, string.Empty);
        if (DiaryInfo == null)
            return (false, DateTime.MinValue, string.Empty);

        var tempData = await DiaryTemporaryService.GetTemporaryDiary(User, DiaryInfo);

        return tempData;
    }
}