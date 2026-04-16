<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load "./../*.linq"

public class SingleHouseBoard : IBoard
{
    public ICell[][] Grid { get; }
    public IHouse[] Houses { get; }
    public IHouse[] Blocks { get; }
    public IHouse[] Rows { get; }
    public IHouse[] Columns { get; }
    
    public SingleHouseBoard(IHouse house)
    {
        Grid = [house.Cells];
        Houses = [house];
        Blocks = [house];
        Rows = [house];
        Columns = [house];
    }
}

public class TestHouse : IHouse
{
    public ICell[] Cells { get; }
    public int[] RemainNumbers => Cells.SelectMany(c => c.Candidate).Distinct().ToArray();
    
    public TestHouse(ICell[] cells)
    {
        Cells = cells;
    }
}

public class Cell : ICell
{
    public int Row { get; }
    public int Column { get; }
    public int Block { get; }
    public int? Number { get; }
    public List<int> Candidate { get; }

    public Cell(int? number, params int[] candidate)
    {
        Number = number;
        Candidate = candidate.ToList();
    }
    
    public static Cell Filled(int number)
    {
        return new Cell(number, []);
    }
    
    public static Cell Empty(params int[] candidate)
    {
        return new Cell(null, candidate);
    }
}