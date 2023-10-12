
namespace GameLibra.Page;

public partial class LibraBoard : JkwPageBase
{
    [Parameter] public string GameId { get; set; }
    [Inject] public ILibraService LibraService { get; set; }

    LibraGameState _state;
    List<DropCubeItem> _cubes;
    protected override void OnPageInitialized()
    {
        _state = LibraService.GetGame(GameId);

        _cubes = GetCubes(_state);
    }

    private List<DropCubeItem> GetCubes(LibraGameState state)
    {
        return state.Players
            .SelectMany(p => GetCubes(p))
            .ToList();
    }
    private List<DropCubeItem> GetCubes(Player player)
    {
        return player.Cubes
            .Select((cube, i) => new DropCubeItem
            {
                Cube = cube,
                Origin = $"player-{player.Id}",
                Identifier = $"player-{player.Id}",
            })
            .ToList();
    }
    private bool CanDrop(DropCubeItem item, string identifier)
    {
        if (item.Identifier.Contains("player"))
        {
            return identifier.Contains("scale");
        }
        else if (item.Identifier.Contains("scale"))
        {
            return item.Origin == identifier;
        }
        else
        {
            return false;
        }
    }
    private void ItemDropped(MudItemDropInfo<DropCubeItem> dropItem)
    {
        dropItem.Item.Identifier = dropItem.DropzoneIdentifier;
    }
}


