using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibra.Page;

public partial class LibraBoardButtons : JkwPageBase
{
    [Inject] public ISnackbar Snackbar { get; set; }
    [Inject] public IDialogService DialogService { get; set; }
    [Parameter] public GameEngine GameEngine { get; set; }
    [Parameter] public LibraGameState State { get; set; }
    [Parameter] public Player CurrentPlayer { get; set; }
    [Parameter] public List<DropCubeItem> Cubes { get; set; }
    [Parameter] public LibraBoardSetting Setting { get; set; }
    [Parameter] public EventCallback<LibraBoardSetting> SettingChanged { get; set; }
    [Parameter] public string RemainTimeText { get; set; }

    private bool CanGuess => 
        State.Scales[0].Left.Value == State.Scales[0].Right.Value
        && State.Scales[0].Left.Value != 0;
    private void Start()
    {
        try
        {
            GameEngine.Start();
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
            var cubeInScale = Cubes
                .Where(x => x.Identifier.Contains("scale"))
                .Select(x => new
                {
                    PlayerId = int.Parse(x.Origin.Split('-')[1]),
                    ScaleId = int.Parse(x.Identifier.Split('-')[1]),
                    Side = x.Identifier.Split('-')[2],
                    Cube = x.Cube,
                    DropItem = x,
                })
                .Where(x => x.PlayerId == State.CurrentPlayerId)
                .ToList();

            var player = State.Players.First(p => p.Id == State.CurrentPlayerId);
            if (player.HasCube(cubeInScale.Select(x => x.Cube)))
            {
                var scaleIds = cubeInScale.Select(x => x.ScaleId).Distinct().ToArray();
                var scaleAndCubes = scaleIds
                    .Select(scaleId =>
                    {
                        var scale = State.Scales.First(x => x.Id == scaleId);
                        var left = cubeInScale.Where(x => x.ScaleId == scaleId && x.Side == "left").Select(x => x.Cube).ToList();
                        var right = cubeInScale.Where(x => x.ScaleId == scaleId && x.Side == "right").Select(x => x.Cube).ToList();
                        return (scale, left, right);
                    })
                    .ToList();
                GameEngine.DoAction(player, scaleAndCubes);
            }

            //foreach (var dropItem in cubeInScale)
            //{
            //    _cubes.Remove(dropItem.DropItem);
            //}

            GameEngine.EndTurn();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error, options =>
            {
                options.VisibleStateDuration = 3000;
            });
        }
    }
    private async Task Guess()
    {
        try
        {
            var param = new DialogParameters
            {
                ["State"] = State,
            };

            var options = new DialogOptions { CloseOnEscapeKey = true };
            var dialog = DialogService.Show<LibraBoardGuessingDialog>("정답 제출", param, options);
            var result = await dialog.Result;

            if (result.Canceled)
            {
                return;
            }

            if (result.Data is List<Cube> guessingData)
            {
                GameEngine.Guess(CurrentPlayer, guessingData);
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error, options =>
            {
                options.VisibleStateDuration = 3000;
            });
        }
    }

    private async Task OpenSetting()
    {
        var param = new DialogParameters
        {
            ["Setting"] = Setting,
        };
        var options = new DialogOptions { CloseOnEscapeKey = true };
        var dialog = DialogService.Show<LibraBoardSettingDialogComponent>("화면 구성", param, options);
        var result = await dialog.Result;

        if (result.Canceled)
        {
            return;
        }

        if (result.Data is LibraBoardSetting setting)
        {
            Setting = setting;
            if (SettingChanged.HasDelegate)
            {
                await SettingChanged.InvokeAsync(Setting);
            }
        }
    }

}
