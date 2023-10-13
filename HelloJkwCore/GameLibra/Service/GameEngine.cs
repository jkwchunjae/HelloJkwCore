namespace GameLibra;

public class GameEngine
{
    public LibraGameState State { get; set; }
    public List<HistoryItem> History { get; set; } = new();

    public event EventHandler<LibraGameState> StateChanged;

    public void Start()
    {
        if (State.Players.Any(x => x.LinkedUser == null))
        {
            throw new Exception("모든 플레이어가 링크되지 않았습니다.");
        }

        State.Status = LibraGameStatus.Playing;
        State.CurrentPlayerId = State.Players.First().Id;

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

        foreach (var (scale, left, right) in scaleAndCube)
        {
            foreach (var cube in left.Concat(right))
            {
                player.Cubes.Remove(cube);
            }
            scale.Left.Add(left);
            scale.Right.Add(right);
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
        }
        else
        {
            State.CurrentPlayerId = -1;
            State.Status = LibraGameStatus.Failed;
        }

        StateChanged?.Invoke(this, State);
    }

    public void LinkPlayer(Player player, AppUser user)
    {
        player.LinkedUser = user;

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
