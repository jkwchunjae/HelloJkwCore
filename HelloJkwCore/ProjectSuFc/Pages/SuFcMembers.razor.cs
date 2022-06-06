namespace ProjectSuFc.Pages;

public partial class SuFcMembers : JkwPageBase
{
    [Inject] ISuFcService Service { get; set; }
    [Inject] IDialogService DialogService { get; set; }

    List<Member> Members { get; set; } = new();

    string NewMemberName = string.Empty;

    protected override async Task OnPageInitializedAsync()
    {
        Members = await Service.GetAllMember();
    }

    private async Task AddNewMember(string memberNameText)
    {
        if (string.IsNullOrWhiteSpace(memberNameText))
            return;

        if (memberNameText.Contains(' '))
            return;

        if (Path.GetInvalidFileNameChars().Any(chr => memberNameText.Contains(chr)))
            return;

        var memberName = new MemberName(memberNameText);

        await Service.SaveMember(new Member()
        {
            Name = memberName,
        });

        NewMemberName = string.Empty;
        Members = await Service.GetAllMember();
    }

    private async Task DeleteAction(Member member)
    {
        var param = new DialogParameters
        {
            ["Content"] = $"{member.Name}님을 삭제하시겠습니까?",
            ["SubmitText"] = "삭제",
            ["SubmitColor"] = Color.Error,
        };
        var dialog = DialogService.Show<SuFcConfirmDialog>("수FC 회원 관리", param);
        var result = await dialog.Result;

        if (result.Data is bool deleteMember && deleteMember)
        {
            await DeleteMember(member);
        }
    }

    private async Task DeleteMember(Member member)
    {
        await Service.DeleteMember(member.Name);
        Members = await Service.GetAllMember();
    }
}
