using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;

namespace ProjectWorldCup.Pages.Betting;

public partial class BettingTableComponent : JkwPageBase
{
    [Parameter]
    public List<WcBettingItem<GroupTeam>> BettingItems { get; set; }
    [Parameter] public bool Visible { get; set; } = false;

    public BettingTableComponent()
    {
    }
}
