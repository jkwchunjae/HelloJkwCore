namespace ProjectWorldCup.Pages.Wc2026;

public partial class BettingSummary2026Component : JkwPageBase
{
    [Parameter]
    public List<UserResult> SummaryResults { get; set; } = new();
}
