
namespace GameLibra.Page;

public partial class LibraBoard : JkwPageBase
{
    [Parameter] public string GameId { get; set; }
    [Inject] public ILibraService LibraService { get; set; }

    GameEngine _gameEngine;
    LibraGameState _state;
    List<DropCubeItem> _cubes;
    protected override Task OnPageInitializedAsync()
    {
        if (IsAuthenticated)
        {
            _gameEngine = LibraService.GetGame(GameId);
            if (_gameEngine == null)
            {
                Navi.NavigateTo("/game/libra/home");
                return Task.CompletedTask;
            }
            _state = _gameEngine?.State;

            _cubes = GetCubes(_state);
        }
        else
        {
            Navi.NavigateTo("/login");
        }
        return base.OnPageInitializedAsync();
    }

    private List<DropCubeItem> GetCubes(LibraGameState state)
    {
        return state?.Players
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
    private void Confirm()
    {
        var cubeInScale = _cubes
            .Where(x => x.Identifier.Contains("scale"))
            .Select(x => new
            {
                ScaleId = int.Parse(x.Identifier.Split('-')[1]),
                Side = x.Identifier.Split('-')[2],
                Cube = x.Cube,
                DropItem = x,
            })
            .ToList();

        var player = _state.Players[_state.TurnPlayerIndex];
        if (player.HasCube(cubeInScale.Select(x => x.Cube)))
        {
            var scaleIds = cubeInScale.Select(x => x.ScaleId).Distinct().ToArray();
            foreach (var scaleId in scaleIds)
            {
                var scale = _state.Scales.First(x => x.Id == scaleId);
                var left = cubeInScale.Where(x => x.ScaleId == scaleId && x.Side == "left").Select(x => x.Cube).ToList();
                var right = cubeInScale.Where(x => x.ScaleId == scaleId && x.Side == "right").Select(x => x.Cube).ToList();
                _gameEngine.DoAction(player, scale, left, right);
            }
        }

        foreach (var dropItem in cubeInScale)
        {
            _cubes.Remove(dropItem.DropItem);
        }

        _gameEngine.EndTurn();
        StateHasChanged();
    }
}


