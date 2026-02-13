namespace ProjectWorldCup.Pages.Wc2022;

public partial class BettingSummary2022Component : JkwPageBase
{
    [Parameter]
    public List<User2022Result> SummaryResults { get; set; } = new();
}
