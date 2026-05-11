namespace ProjectWorldCup.Pages.Wc2022;

public partial class BettingSummary2022Component : JkwPageBase
{
    [Parameter]
    public List<UserResult> SummaryResults { get; set; } = new();
}
