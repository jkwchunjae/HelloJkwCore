
namespace GameLibra.Page;

public partial class LibraBoard : JkwPageBase
{
    LibraGameState state;
    List<DropCubeItem> _cubes;
    protected override void OnPageInitialized()
    {
        state = new GameStateBuilder()
            .UseDevilsPlanRule()
            .Build();

        var scale = state.Scales[0];
        scale.Left.Add(state.CubeInfo[0]);
        scale.Left.Add(state.CubeInfo[1]);
        scale.Left.Add(state.CubeInfo[0]);
        scale.Left.Add(state.CubeInfo[0]);
        scale.Left.Add(state.CubeInfo[0]);
        scale.Right.Add(state.CubeInfo[2]);
        scale.Right.Add(state.CubeInfo[1]);

        _cubes = GetCubes();
    }

    private List<DropCubeItem> GetCubes()
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


