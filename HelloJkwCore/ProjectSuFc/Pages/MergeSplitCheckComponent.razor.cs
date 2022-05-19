namespace ProjectSuFc.Pages;

public partial class MergeSplitCheckComponent : JkwPageBase
{
    [Parameter] public List<Member> Members { get; set; }
    [Parameter] public MergeSplitOption MergeSplitOption { get; set; }
    [Parameter] public EventCallback<Member> Checked { get; set; }
    [Parameter] public EventCallback<Member> Unchecked { get; set; }
    [Parameter] public Func<string, string> MessageTemplate { get; set; }

    private async Task Uncheck(Member member)
    {
        await Unchecked.InvokeAsync(member);
    }

    private async Task Check(Member member)
    {
        await Checked.InvokeAsync(member);
    }
}
