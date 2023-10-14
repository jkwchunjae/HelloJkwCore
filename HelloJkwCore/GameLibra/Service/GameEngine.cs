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
            throw new Exception("��� �÷��̾ ������� �ʾҽ��ϴ�.");
        }

        State.Status = LibraGameStatus.Playing;
        State.CurrentPlayerId = State.Players.First().Id;

        StateChanged?.Invoke(this, State);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="player">�ൿ�� ��ü</param>
    /// <param name="cubes">�������￡ �ø� ť�� ��</param>
    /// <param name="scale">Ÿ�� ��������</param>
    public void DoAction(Player player, List<(DoubleScale Scale, List<Cube> Left, List<Cube> Right)> scaleAndCube)
    {
        if (State.Status != LibraGameStatus.Playing)
        {
            throw new Exception("������ ���۵��� �ʾҽ��ϴ�.");
        }

        Cube[] cubes = scaleAndCube.SelectMany(x => x.Left)
            .Concat(scaleAndCube.SelectMany(x => x.Right))
            .ToArray();
        DoubleScale[] scales = scaleAndCube.Select(x => x.Scale).ToArray();
        if (!player.HasCube(cubes))
        {
            throw new Exception("�÷��̾ ť�긦 ������ ���� �ʽ��ϴ�.");
        }
        if (State.Players.All(x => x.Id != player.Id))
        {
            throw new Exception("�÷��̾ ���ӿ� �������� �ʾҽ��ϴ�.");
        }

        foreach (var (scale, left, right) in scaleAndCube)
        {
            if (!State.Scales.Contains(scale))
            {
                throw new Exception("Ÿ�� ���������� �������� �ʽ��ϴ�.");
            }
        }
        if (cubes.Count() < State.Rule.MinimumApplyCubeCount)
        {
            throw new Exception($"��� {State.Rule.MinimumApplyCubeCount}�� �̻��� ť�긦 �÷��� �մϴ�.");
        }

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
            State.ResultMessage = "��� �÷��̾ ť�긦 �� ����߽��ϴ�.";
        }

        StateChanged?.Invoke(this, State);
    }

    public void LinkPlayer(Player player, AppUser user)
    {
        player.LinkedUser = user;

        StateChanged?.Invoke(this, State);
    }

    public void Guess(Player player, List<Cube> guessing)
    {
        var result = State.CubeInfo
            .All(goalCube => guessing.Any(guessCube => guessCube.Id == goalCube.Id && guessCube.Value == goalCube.Value));

        if (result)
        {
            State.Status = LibraGameStatus.Success;
            State.ResultMessage = $"{player.LinkedUser.DisplayName}���� ���� ����!";
        }
        else
        {
            State.Status = LibraGameStatus.Failed;
            State.ResultMessage = $"{player.LinkedUser.DisplayName}���� ������ �����߽��ϴ�.";
        }

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
