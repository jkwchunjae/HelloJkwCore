namespace ProjectSuFc.Pages;

public partial class SuFcMembers : JkwPageBase
{
    [Inject] ISuFcService Service { get; set; }

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
}
