using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;

namespace ProjectWorldCup.Pages.Betting;

public partial class BettingTableComponent : JkwPageBase
{
    [Inject]
    public IJSRuntime JS { get; set; }
    [Parameter]
    public List<WcBettingItem<GroupTeam>> BettingItems { get; set; }

    public BettingTableComponent()
    {
    }
}
