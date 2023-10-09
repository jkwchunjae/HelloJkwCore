namespace GameLibra;

public class LibraGameState
{
    public string Id { get; set; }
    public string Name { get; set; }
    public LibraGameRule Rule { get; set; }
    public List<Cube> CubeInfo { get; set; }
    public List<Player> Players { get; set; }
    public List<DoubleScale> Scales { get; set; }
    public int TurnPlayerIndex { get; set; }
}

public class DoubleScale
{
    public SingleArm Left { get; set; } = new();
    public SingleArm Right { get; set; } = new();
}

public class SingleArm
{
    public List<Cube> Cubes { get; set; } = new();

    public int Value => Cubes?.Sum(c => c.Value) ?? 0;

    public void Add(Cube cube)
    {
        Cubes ??= new List<Cube>();
        Cubes.Add(cube);
    }
}

public class Player
{
    public int Id { get; set; }
    public List<CubeCount> Cubes { get; set; }
}

public class CubeCount
{
    public Cube Cube { get; set; }
    public int Count { get; set; }
}

public class Cube
{
    public int Id { get; set; }
    public int Value { get; set; }
}