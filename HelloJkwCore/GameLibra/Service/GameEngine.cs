namespace GameLibra;

public class GameEngine
{
    public LibraGameState State { get; set; }
    public List<HistoryItem> History { get; set; } = new();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="player">�ൿ�� ��ü</param>
    /// <param name="cubes">�������￡ �ø� ť�� ��</param>
    /// <param name="scale">Ÿ�� ��������</param>
    public void DoAction(Player player, DoubleScale scale, List<Cube> left, List<Cube> right)
    {
        var cubes = (left ?? Enumerable.Empty<Cube>())
            .Concat(right ?? Enumerable.Empty<Cube>())
            .ToArray();
        if (!player.HasCube(cubes))
        {
            throw new Exception("�÷��̾ ť�긦 ������ ���� �ʽ��ϴ�.");
        }
        if (State.Players.All(x => x.Id != player.Id))
        {
            throw new Exception("�÷��̾ ���ӿ� �������� �ʾҽ��ϴ�.");
        }
        if (State.Scales.All(x => x != scale))
        {
            throw new Exception("Ÿ�� ���������� �������� �ʽ��ϴ�.");
        }

        var playerCubes = player.Cubes.ToList();
        foreach (var cube in cubes)
        {
            // Ȥ�� ���� �� �� �� üũ
            var removeCube = playerCubes.First(x => x.Id == cube.Id);
            if (removeCube == null)
                throw new Exception("�÷��̾ �ش� ť�긦 ������ ���� �ʽ��ϴ�.");
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
