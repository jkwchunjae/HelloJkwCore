namespace GameLibra;

public class GameEngine
{
    public LibraGameState State { get; set; }
    public List<HistoryItem> History { get; set; } = new();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="player">행동의 주체</param>
    /// <param name="cubes">양팔저울에 올릴 큐브 수</param>
    /// <param name="scale">타겟 양팔저울</param>
    public void DoAction(Player player, DoubleScale scale, List<Cube> left, List<Cube> right)
    {
        var cubes = (left ?? Enumerable.Empty<Cube>())
            .Concat(right ?? Enumerable.Empty<Cube>())
            .ToArray();
        if (!player.HasCube(cubes))
        {
            throw new Exception("플레이어가 큐브를 가지고 있지 않습니다.");
        }
        if (State.Players.All(x => x.Id != player.Id))
        {
            throw new Exception("플레이어가 게임에 참여하지 않았습니다.");
        }
        if (State.Scales.All(x => x != scale))
        {
            throw new Exception("타겟 양팔저울이 존재하지 않습니다.");
        }

        var playerCubes = player.Cubes.ToList();
        foreach (var cube in cubes)
        {
            // 혹시 몰라 한 번 더 체크
            var removeCube = playerCubes.First(x => x.Id == cube.Id);
            if (removeCube == null)
                throw new Exception("플레이어가 해당 큐브를 가지고 있지 않습니다.");
            playerCubes.Remove(removeCube);
        }
        player.Cubes = playerCubes.ToArray();
        scale.Left.Add(left);
        scale.Right.Add(right);
    }

    public void EndTurn()
    {
        State.TurnPlayerIndex = (State.TurnPlayerIndex + 1) % State.Players.Count;
    }
}

public class HistoryItem
{
    public int PlayerId { get; set; }
    public string TargetScaleIndex { get; set; }
    public Cube[] Left { get; set; }
    public Cube[] Right { get; set; }
}
