using GameLibra.Service;

namespace GameLibra;

public class GameEngine
{
    public LibraGameState State { get; set; }
    public List<HistoryItem> History { get; set; } = new();
    public LibraAssistor Assistor { get; set; }

    private TimeOverHandler _timeOverHandler = new();

    public event EventHandler<LibraGameState> StateChanged;
    public event EventHandler<RemainTime> RemainTimeChanged;

    public GameEngine()
    {
        _timeOverHandler.RemainTimeChanged += (sender, e) => RemainTimeChanged?.Invoke(sender, e);
        _timeOverHandler.TimeOver += (sender, player) => OnTimeOver(player);
    }

    public void Start()
    {
        if (State.Players.Any(x => x.LinkedUser == null))
        {
            throw new Exception("모든 플레이어가 연결되지 않았습니다.");
        }

        State.Status = LibraGameStatus.Playing;
        State.CurrentPlayerId = State.Players.First().Id;

        _timeOverHandler.StartNew(State.Players.First(), TimeSpan.FromSeconds(State.Rule.TimeOverSeconds));

        StateChanged?.Invoke(this, State);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="player">행동의 주체</param>
    /// <param name="cubes">양팔저울에 올릴 큐브 수</param>
    /// <param name="scale">타겟 양팔저울</param>
    public void DoAction(Player player, List<(DoubleScale Scale, List<Cube> Left, List<Cube> Right)> scaleAndCube)
    {
        if (State.Status != LibraGameStatus.Playing)
        {
            throw new Exception("게임이 시작되지 않았습니다.");
        }

        Cube[] cubes = scaleAndCube.SelectMany(x => x.Left)
            .Concat(scaleAndCube.SelectMany(x => x.Right))
            .ToArray();
        DoubleScale[] scales = scaleAndCube.Select(x => x.Scale).ToArray();
        if (!player.HasCube(cubes))
        {
            throw new Exception("플레이어가 큐브를 가지고 있지 않습니다.");
        }
        if (State.Players.All(x => x.Id != player.Id))
        {
            throw new Exception("플레이어가 게임에 참여하지 않았습니다.");
        }

        foreach (var (scale, left, right) in scaleAndCube)
        {
            if (!State.Scales.Contains(scale))
            {
                throw new Exception("타겟 양팔저울이 존재하지 않습니다.");
            }
        }
        if (cubes.Count() < State.Rule.MinimumApplyCubeCount)
        {
            throw new Exception($"적어도 {State.Rule.MinimumApplyCubeCount}개 이상의 큐브를 올려야 합니다.");
        }

        _timeOverHandler.Clear();

        foreach (var scale in State.Scales)
        {
            foreach (var cube in scale.Left.Cubes)
            {
                cube.New = false;
            }
            foreach (var cube in scale.Right.Cubes)
            {
                cube.New = false;
            }
        }
        if (State.UseAssist)
        {
            foreach (var scale in State.Scales.Where(x => x.Left.Value == x.Right.Value))
            {
                scale.Left.Cubes.Clear();
                scale.Right.Cubes.Clear();
            }
        }

        foreach (var (scale, left, right) in scaleAndCube)
        {
            foreach (var cube in left.Concat(right))
            {
                player.Cubes.Remove(cube);
                cube.New = true;
            }
            var stateScale = State.Scales.First(x => x.Id == scale.Id);
            stateScale.Left.Add(left);
            stateScale.Right.Add(right);
        }

        if (State.UseAssist)
        {
            foreach (var scale in State.Scales)
            {
                foreach (var cube in State.CubeInfo)
                {
                    var leftCount = scale.Left.Cubes.Count(c => c.Id == cube.Id);
                    var rightCount = scale.Right.Cubes.Count(c => c.Id == cube.Id);

                    if (leftCount > 0 && rightCount > 0)
                    {
                        var removeCount = Math.Min(leftCount, rightCount);
                        for (var i = 0; i < removeCount; i++)
                        {
                            scale.Left.Cubes.Remove(scale.Left.Cubes.First(c => c.Id == cube.Id));
                            scale.Right.Cubes.Remove(scale.Right.Cubes.First(c => c.Id == cube.Id));
                        }
                    }
                }
            }
        }

        foreach (var scale in State.Scales)
        {
            var leftCubes = scale.Left.Cubes.Select(x => x.Name).ToArray();
            var rightCubes = scale.Right.Cubes.Select(x => x.Name).ToArray();
            if (scale.Left.Value == scale.Right.Value)
            {
                Assistor.SameValue(leftCubes, rightCubes);
            }
            else if (scale.Left.Value < scale.Right.Value)
            {
                Assistor.LessThan(leftCubes, rightCubes);
            }
            else
            {
                Assistor.GreaterThan(leftCubes, rightCubes);
            }
        }

        StateChanged?.Invoke(this, State);
    }

    public void EndTurn()
    {
        var currentPlayerIndex = State.Players.FindIndex(p => p.Id == State.CurrentPlayerId);

        var found = false;
        var nextPlayerIndex = 0;
        for (var i = 0; i < State.Players.Count; i++)
        {
            nextPlayerIndex = (currentPlayerIndex + i + 1) % State.Players.Count;
            var player = State.Players[nextPlayerIndex];
            if (player.Cubes.Count >= State.Rule.MinimumApplyCubeCount)
            {
                found = true;
                break;
            }
        }
        if (found)
        {
            var nextPlayer = State.Players[nextPlayerIndex];
            State.CurrentPlayerId = State.Players[nextPlayerIndex].Id;

            _timeOverHandler.StartNew(nextPlayer, TimeSpan.FromSeconds(State.Rule.TimeOverSeconds));
        }
        else
        {
            State.CurrentPlayerId = -1;
            State.Status = LibraGameStatus.Failed;
            State.ResultMessage = "모든 플레이어가 큐브를 다 사용했습니다.";
        }

        StateChanged?.Invoke(this, State);
    }

    public void LinkPlayer(Player player, AppUser user)
    {
        if (player.LinkedUser != null)
        {
            throw new Exception("이미 플레이어가 연결되어 있습니다.");
        }
        player.LinkedUser = user;

        StateChanged?.Invoke(this, State);
    }

    public void Guess(Player player, List<Cube> guessing)
    {
        var result = State.CubeInfo
            .All(goalCube => guessing.Any(guessCube => guessCube.Id == goalCube.Id && guessCube.Value == goalCube.Value));
        var goalValues = State.CubeInfo
            .OrderBy(x => x.Value)
            .Select(x => x.Value.ToString())
            .StringJoin(", ");
        var gussingValues = guessing
            .OrderBy(x => x.Value)
            .Select(x => x.Value.ToString())
            .StringJoin(", ");

        _timeOverHandler.Clear();

        if (result)
        {
            State.Status = LibraGameStatus.Success;
            State.ResultMessage = $"{player.LinkedUser.DisplayName}님의 추측 성공! ({gussingValues})";
        }
        else
        {
            State.Status = LibraGameStatus.Failed;
            State.ResultMessage = $"{player.LinkedUser.DisplayName}님이 추측에 실패했습니다. (추측: {gussingValues}, 정답: {goalValues})";
        }

        StateChanged?.Invoke(this, State);
    }

    private void OnTimeOver(Player player)
    {
        try
        {
            State.Status = LibraGameStatus.Failed;
            State.ResultMessage = $"{player.LinkedUser.DisplayName}님이 시간 초과했습니다.";

            StateChanged?.Invoke(this, State);
        }
        catch
        {
        }
    }

    public void UseAssist()
    {
        State.UseAssist = true;
        StateChanged?.Invoke(this, State);
    }
}

public class HistoryItem
{
    public int PlayerId { get; set; }
    public string TargetScaleIndex { get; set; }
    public Cube[] Left { get; set; }
    public Cube[] Right { get; set; }
}