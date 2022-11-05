using Microsoft.JSInterop;

namespace ProjectWorldCup.Pages;

public partial class GroupStage : JkwPageBase
{
    [Inject]
    private IWorldCupService Service { get; set; }

    private List<SimpleGroup> Groups { get; set; } = new();

    protected override async Task OnPageInitializedAsync()
    {
        Groups = await Service.GetSimpleGroupsAsync();
        //await Js.InvokeVoidAsync("console.log", Groups);
    }
}