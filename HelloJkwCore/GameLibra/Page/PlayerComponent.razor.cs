
namespace GameLibra.Page;

public partial class PlayerComponent : JkwPageBase
{
    [Parameter]
    public Player Player { get; set; }

    private string CubeIdentifier => $"player-{Player.Id}";
}
