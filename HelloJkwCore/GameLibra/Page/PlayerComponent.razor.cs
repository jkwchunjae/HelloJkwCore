
namespace GameLibra.Page;

public partial class PlayerComponent : JkwPageBase
{
    [Parameter] public GameEngine GameEngine { get; set; }
    [Parameter] public Player Player { get; set; }
    [Parameter] public EventCallback<Player> PlayerChanged { get; set; }
    [Parameter] public int CurrentPlayerId { get; set; }

    private string CubeIdentifier => $"player-{Player.Id}";

    private void LinkPlayer()
    {
        GameEngine.LinkPlayer(Player, User);
    }
}
