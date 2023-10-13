
namespace GameLibra.Page;

public partial class LibraBoard : JkwPageBase
{
    [Parameter] public string GameId { get; set; }
    [Inject] public ILibraService LibraService { get; set; }
    [Inject] public ISnackbar Snackbar { get; set; }

    GameEngine _gameEngine;
    LibraGameState _state;
    List<DropCubeItem> _cubes;
    Player _currentPlayer;
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
            _gameEngine.StateChanged += GameEngine_StateChanged;
            _state = _gameEngine?.State;
            _cubes = GetCubes(_state);
        }
        else
        {
            Navi.NavigateTo("/login");
        }
        return base.OnPageInitializedAsync();
    }

    private void GameEngine_StateChanged(object sender, LibraGameState e)
    {
        InvokeAsync(() =>
        {
            _state = e;
            _cubes = GetCubes(_state);
            _currentPlayer = _state.Players.FirstOrDefault(x => x.Id == _state.CurrentPlayerId);
            StateHasChanged();
        });
    }

    protected override void OnPageDispose()
    {
        if (_gameEngine != null)
        {
            _gameEngine.StateChanged -= GameEngine_StateChanged;
        }
        base.OnPageDispose();
    }

    private List<DropCubeItem> GetCubes(LibraGameState state)
    {
        return state?.Players
            .SelectMany(p => GetCubes(p))
            .ToList();

        List<DropCubeItem> GetCubes(Player player)
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
    }
    private bool CanDrop(DropCubeItem item, string targetIdentifier)
    {
        if (item.Identifier.Contains("player"))
        {
            return targetIdentifier.Contains("scale");
        }
        else if (item.Identifier.Contains("scale"))
        {
            if (targetIdentifier.Contains("scale"))
            {
                return true;
            }
            else
            {
                return item.Origin == targetIdentifier;
            }
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
    private void Start()
    {
        try
        {
            _gameEngine.Start();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error, options =>
            {
                options.VisibleStateDuration = 3000;
            });
        }
    }
    private void Confirm()
    {
        try
        {
            var cubeInScale = _cubes
                .Where(x => x.Identifier.Contains("scale"))
                .Select(x => new
                {
                    PlayerId = int.Parse(x.Origin.Split('-')[1]),
                    ScaleId = int.Parse(x.Identifier.Split('-')[1]),
                    Side = x.Identifier.Split('-')[2],
                    Cube = x.Cube,
                    DropItem = x,
                })
                .Where(x => x.PlayerId == _state.CurrentPlayerId)
                .ToList();

            var player = _state.Players.First(p => p.Id == _state.CurrentPlayerId);
            if (player.HasCube(cubeInScale.Select(x => x.Cube)))
            {
                var scaleIds = cubeInScale.Select(x => x.ScaleId).Distinct().ToArray();
                var scaleAndCubes = scaleIds
                    .Select(scaleId =>
                    {
                        var scale = _state.Scales.First(x => x.Id == scaleId);
                        var left = cubeInScale.Where(x => x.ScaleId == scaleId && x.Side == "left").Select(x => x.Cube).ToList();
                        var right = cubeInScale.Where(x => x.ScaleId == scaleId && x.Side == "right").Select(x => x.Cube).ToList();
                        return (scale, left, right);
                    })
                    .ToList();
                _gameEngine.DoAction(player, scaleAndCubes);
            }

            foreach (var dropItem in cubeInScale)
            {
                _cubes.Remove(dropItem.DropItem);
            }

            _gameEngine.EndTurn();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error, options =>
            {
                options.VisibleStateDuration = 3000;
            });
        }
    }
}


