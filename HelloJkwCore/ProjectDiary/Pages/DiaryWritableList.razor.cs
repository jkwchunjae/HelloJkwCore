namespace ProjectDiary.Pages;

public partial class DiaryWritableList : JkwPageBase
{
    [Inject] private IDiaryService DiaryService { get; set; }

    private UserDiaryInfo UserDiaryInfo;

    protected override async Task OnPageInitializedAsync()
    {
        if (IsAuthenticated)
        {
            UserDiaryInfo = await DiaryService.GetUserDiaryInfoAsync(User);
        }
    }
}
