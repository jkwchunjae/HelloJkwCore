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

public class MultipleHouseBoard : IBoard
{
    public ICell[][] Grid { get; }
    public IHouse[] Houses { get; }
    public IHouse[] Blocks { get; }
    public IHouse[] Rows { get; }
    public IHouse[] Columns { get; }

    public MultipleHouseBoard(IHouse[] house)
    {
        Grid = house.SelectMany(h => h.Cells)
            .GroupBy(cell => cell.Row)
            .Select(group => group.OrderBy(c => c.Column).ToArray())
            .ToArray();
        Houses = house;
        Blocks = house;
        Rows = house;
        Columns = house;
    }
}

public class Board : IBoard
{
    public ICell[][] Grid { get; }
    public IHouse[] Houses { get; }
    public IHouse[] Blocks { get; }
    public IHouse[] Rows { get; }
    public IHouse[] Columns { get; }

    public Board(ICell[][] grid)
    {
        Grid = grid;
        Rows = MakeRowHouse(grid);
        Columns = MakeColumnHouse(grid);
        Blocks = MakeBlockHouse(grid);
        Houses = Rows
            .Concat(Columns)
            .Concat(Blocks)
            .ToArray();
    }
    
    private IHouse[] MakeRowHouse(ICell[][] grid)
    {
        return grid
            .Select(row => new TestHouse(row))
            .ToArray();
    }
    
    private IHouse[] MakeColumnHouse(ICell[][] grid)
    {
        return grid
            .SelectMany(x => x)
            .GroupBy(x => x.Column)
            .Select(g => g.OrderBy(c => c.Row).ToArray())
            .Select(column => new TestHouse(column))
            .ToArray();
    }

    private IHouse[] MakeBlockHouse(ICell[][] grid)
    {
        return grid
            .SelectMany(x => x)
            .GroupBy(x => x.Block)
            .Select(g => g.OrderBy(c => c.Row).ThenBy(c => c.Column).ToArray())
            .Select(block => new TestHouse(block))
            .ToArray();
    }
}

public class TestHouse : IHouse
{
    public ICell[] Cells { get; }
    public int[] RemainNumbers => Cells
        .SelectMany(c => c.Candidate)
        .Distinct()
        .ToArray();
    
    public TestHouse(ICell[] cells)
    {
        Cells = cells;
    }
}

public class Cell : ICell
{
    public int Row { get; }
    public int Column { get; }
    public int Block => ((Row - 1) / 3) * 3 + ((Column - 1) / 3) + 1;
    public int? Number { get; }
    public List<int> Candidate { get; }
    
    public Cell(int row, int column)
    {
        Row = row;
        Column = column;
        Candidate = new();
    }

    public Cell(int row, int column, int number)
        : this(row, column)
    {
        Number = number;
    }
    
    public Cell(int row, int column, params int[] candidate)
        : this(row, column)
    {
        Number = null;
        Candidate = candidate.ToList();
    }

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

public class BoardBuilder
{
    int?[][]? _numbers;
    Dictionary<(int row, int column), int> setNumbers = new();
    Dictionary<(int row, int column), int[]> setCandidates = new();

    /// <summary>
    /// numbers: 9*9 숫자가 써있음.
    ///         숫자가 정해지지 않았으면 공백으로 처리.
    /// </summary>
    public BoardBuilder SetNumbers(string numbers)
    {
        var arr = numbers
            .Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None)
            .ToArray();

        if (arr.Length != 9)
            throw new ArgumentException("numbers는 9행 이어야 합니다.", nameof(numbers));
        if (arr.Any(line => line.Length != 9))
            throw new ArgumentException("numbers는 9열 이어야 합니다.", nameof(numbers));

        _numbers = arr
            .Select(line => line.Select(chr =>
            {
                int? parsed = (chr == ' ' || chr == '.') ? null : int.Parse(chr.ToString());
                return parsed;
            })
            .ToArray())
            .ToArray();
            
        return this;
    }
    
    public BoardBuilder SetNumber(int row, int column, int number)
    {
        setNumbers[(row, column)] = number;
        return this;
    }
    
    public BoardBuilder SetCandidate(int row, int column, params int[] candidate)
    {
        setCandidates[(row, column)] = candidate;
        return this;
    }
    
    public IBoard Build()
    {
        ICell[][] cells = Enumerable.Range(1, 9)
            .Select(row => Enumerable.Range(1, 9)
            .Select(column =>
            {
                if (setNumbers.TryGetValue((row, column), out int number))
                {
                    return new Cell(row, column, number);
                }
                else if (_numbers?[row - 1][column - 1] != null)
                {
                    number = _numbers[row - 1][column - 1]!.Value;
                    return new Cell(row, column, number);
                }
                else if (setCandidates.TryGetValue((row, column), out var candidates))
                {
                    return new Cell(row, column, candidates);
                }
                else
                {
                    return new Cell(row, column);
                }
            })
            .ToArray())
            .ToArray();

        return new Board(cells);
    }
}