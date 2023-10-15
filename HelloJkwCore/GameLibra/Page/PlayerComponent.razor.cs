
namespace GameLibra.Page;

public partial class PlayerComponent : JkwPageBase
{
    [Parameter] public Player Player { get; set; }
    [Parameter] public EventCallback<Player> PlayerChanged { get; set; }
    [Parameter] public int CurrentPlayerId { get; set; }
    [Parameter] public LibraGameRule Rule { get; set; }
    [Parameter] public LibraBoardSetting Setting { get; set; }
    [Parameter] public EventCallback<(Player Player, AppUser User)> PlayerLinked { get; set; }

    private string CubeIdentifier => $"player-{Player.Id}";

    private void LinkPlayer()
    {
        if (PlayerLinked.HasDelegate)
        {
            PlayerLinked.InvokeAsync((Player, User));
        }
    }
}
