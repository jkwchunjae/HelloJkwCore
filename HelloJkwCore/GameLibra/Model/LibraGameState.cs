namespace GameLibra;

public class LibraGameState
{
    public string Id { get; set; }
    public string Name { get; set; }
    public AppUser Owner { get; set; }
    public LibraGameRule Rule { get; set; }
    public List<Cube> CubeInfo { get; set; }
    public List<Player> Players { get; set; }
    public List<DoubleScale> Scales { get; set; }
    public int TurnPlayerIndex { get; set; }
}

public class DoubleScale
{
    public int Id { get; set; }
    public SingleArm Left { get; set; } = new();
    public SingleArm Right { get; set; } = new();
}

public class SingleArm
{
    public List<Cube> Cubes { get; set; } = new();

    public int Value => Cubes?.Sum(c => c.Value) ?? 0;
    public List<Cube> Filter(int id)
        => Cubes.Where(x => x.Id == id).ToList();

    public void Add(Cube cube)
    {
        Cubes.Add(cube);
    }
    public void Add(IEnumerable<Cube> cubes)
    {
        if (cubes?.Any() ?? false)
        {
            Cubes.AddRange(cubes);
        }
    }
}

public class Player
{
    public int Id { get; set; }
    public AppUser LinkedUser { get; set; }
    public Cube[] Cubes { get; set; }

    public bool HasCube(IEnumerable<Cube> cubes)
    {
        var cubeIds = cubes.Select(x => x.Id).Distinct().ToArray();
        foreach (var id in cubeIds)
        {
            var targetCubeCount = cubes.Count(x => x.Id == id);
            var playerCubeCount = Cubes.Count(x => x.Id == id);
            if (playerCubeCount < targetCubeCount)
                return false;
        }
        return true;
    }
}

public class Cube
{
    public int Id { get; set; }
    public int Value { get; set; }
}

public class DropCubeItem
{
    public Cube Cube { get; set; }
    public string Origin { get; set; }
    public string Identifier { get; set; }
}
