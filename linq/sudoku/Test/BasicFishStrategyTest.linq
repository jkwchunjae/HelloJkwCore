<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load "./*.linq"
#load "./../*.linq"

void Main()
{
    XWingRowExample2();
    XWingRowExample3();
    XWingRowExample();
    SwordfishRowExample();
    JellyfishRowExample();
}

void XWingRowExample2()
{
    var board = new BoardBuilder()
        .SetNumbers("""
         41729 3 
        769  34 2
         3264 719
        4 39  17 
        6 7  49 3
        19537  24
        214567398
        376 9 541
        958431267
        """)
        .SetCandidate(1, 1, [5, 8])
        .SetCandidate(1, 7, [6, 8])
        .SetCandidate(1, 9, [5, 6])
        .SetCandidate(2, 4, [1, 8])
        .SetCandidate(2, 5, [1,5,8])
        .SetCandidate(2, 8, [5, 8])
        .SetCandidate(3, 1, [5, 8])
        .SetCandidate(3, 6, [5, 8])
        .SetCandidate(4, 2, [2, 8])
        .SetCandidate(4, 5, [5, 8])
        .SetCandidate(4, 6, [2, 5, 6, 8])
        .SetCandidate(4, 9, [5, 6])
        .SetCandidate(5, 2, [2, 8])
        .SetCandidate(5, 4, [1, 2, 8])
        .SetCandidate(5, 5, [1, 5, 8])
        .SetCandidate(5, 8, [5, 8])
        .SetCandidate(6, 6, [6, 8])
        .SetCandidate(6, 7, [6, 8])
        .SetCandidate(8, 4, [2, 8])
        .SetCandidate(8, 6, [2, 8])
        .Build();
        
    RunStrategy("XWingRowExample2", new XWingFishStrategy(), board);
}

void XWingRowExample3()
{
    var board = new BoardBuilder()
        .SetNumbers("""
        98 62 753
         65  3   
        327 5   6
        79  3 5  
         5   9   
        832 45  9
        673591428
        249 87  5
        518 2   7
        """)
        .SetCandidate(1, 3, [1, 4])
        .SetCandidate(1, 4, [1, 4])
        // 2행 부터 codex가 채워.
        .SetCandidate(2, 1, [1, 4])
        .SetCandidate(2, 4, [1, 4, 7, 8, 9])
        .SetCandidate(2, 5, [1, 7])
        .SetCandidate(2, 7, [1, 2, 8, 9])
        .SetCandidate(2, 8, [1, 4, 8, 9])
        .SetCandidate(2, 9, [1, 2, 4])
        .SetCandidate(3, 4, [1, 4, 8, 9])
        .SetCandidate(3, 6, [4, 8])
        .SetCandidate(3, 7, [1, 8, 9])
        .SetCandidate(3, 8, [1, 4, 8, 9])
        .SetCandidate(4, 3, [1, 4, 6])
        .SetCandidate(4, 4, [1, 2, 6, 8])
        .SetCandidate(4, 6, [6, 8])
        .SetCandidate(4, 8, [1, 4, 6, 8])
        .SetCandidate(4, 9, [1, 2, 4])
        .SetCandidate(5, 1, [1, 4])
        .SetCandidate(5, 3, [1, 4, 6])
        .SetCandidate(5, 4, [1, 2, 6, 7, 8])
        .SetCandidate(5, 5, [1, 7])
        .SetCandidate(5, 7, [1, 2, 3, 6, 8])
        .SetCandidate(5, 8, [1, 3, 4, 6, 7, 8])
        .SetCandidate(5, 9, [1, 2, 4])
        .SetCandidate(6, 4, [1, 6, 7])
        .SetCandidate(6, 7, [1, 6])
        .SetCandidate(6, 8, [1, 6, 7])
        .SetCandidate(8, 4, [3, 6])
        .SetCandidate(8, 7, [1, 3, 6])
        .SetCandidate(8, 8, [1, 3, 6])
        .SetCandidate(9, 4, [3, 4, 6])
        .SetCandidate(9, 6, [4, 6])
        .SetCandidate(9, 7, [3, 6, 9])
        .SetCandidate(9, 8, [3, 6, 9])
        .Build();

    RunStrategy("XWingRowExample3", new XWingFishStrategy(), board);
}

void XWingRowExample()
{
    var board = new FishBoardBuilder()
        // base rows r2 & r5, columns c5 & c8 contain digit 5
        .WithCandidates(2, 5, 5).WithCandidates(2, 8, 5)
        .WithCandidates(5, 5, 5).WithCandidates(5, 8, 5)
        // cover column candidates outside base rows -> eliminations
        .WithCandidates(4, 5, 5).WithCandidates(1, 8, 5)
        .Build();

    RunStrategy("XWingRowExample", new XWingFishStrategy(), board);
}

void SwordfishRowExample()
{
    var board = new FishBoardBuilder()
        // base rows r1, r2, r9; cover columns c1, c5, c8 for digit 2
        .WithCandidates(1, 1, 2).WithCandidates(1, 5, 2)
        .WithCandidates(2, 5, 2).WithCandidates(2, 8, 2)
        .WithCandidates(9, 1, 2).WithCandidates(9, 8, 2)
        // eliminations described in HoDoKu sample
        .WithCandidates(7, 1, 2).WithCandidates(6, 8, 2)
        .Build();

    RunStrategy("SwordfishRowExample", new SwordfishStrategy(), board);
}

void JellyfishRowExample()
{
    var board = new FishBoardBuilder()
        // base rows r3, r4, r6, r7; cover columns c1, c2, c5, c9 for digit 7
        .WithCandidates(3, 1, 7).WithCandidates(3, 2, 7)
        .WithCandidates(4, 5, 7).WithCandidates(4, 9, 7)
        .WithCandidates(6, 1, 7).WithCandidates(6, 5, 7)
        .WithCandidates(7, 2, 7).WithCandidates(7, 9, 7)
        // extra candidates in cover columns to be removed
        .WithCandidates(1, 1, 7).WithCandidates(8, 9, 7)
        .Build();

    RunStrategy("JellyfishRowExample", new JellyfishStrategy(), board);
}

void RunStrategy(string name, ICandidateStrategy strategy, IBoard board)
{
    if (strategy.TryRemoveCandidate(board, out var result))
    {
        result.Dump(name, 1);
    }
    else
    {
        "nothing".Dump(name);
    }
}

public class FishBoardBuilder
{
    private readonly TestCell[,] _cells;

    public FishBoardBuilder()
    {
        _cells = new TestCell[9, 9];
        for (int row = 1; row <= 9; row++)
        {
            for (int column = 1; column <= 9; column++)
            {
                _cells[row - 1, column - 1] = new TestCell(row, column);
            }
        }
    }

    public FishBoardBuilder WithCandidates(int row, int column, params int[] candidates)
    {
        _cells[row - 1, column - 1].AddCandidates(candidates);
        return this;
    }

    public FishTestBoard Build()
    {
        return new FishTestBoard(_cells);
    }
}

public class FishTestBoard : IBoard
{
    public ICell[][] Grid { get; }
    public IHouse[] Houses { get; }
    public IHouse[] Blocks { get; }
    public IHouse[] Rows { get; }
    public IHouse[] Columns { get; }

    public FishTestBoard(TestCell[,] cells)
    {
        Grid = Enumerable.Range(0, 9)
            .Select(r => Enumerable.Range(0, 9)
                .Select(c => (ICell)cells[r, c])
                .ToArray())
            .ToArray();

        Rows = Enumerable.Range(0, 9)
            .Select(r => new TestHouse(Grid[r]))
            .ToArray();

        Columns = Enumerable.Range(0, 9)
            .Select(c => new TestHouse(Enumerable.Range(0, 9).Select(r => Grid[r][c]).ToArray()))
            .ToArray();

        Blocks = Enumerable.Range(0, 9)
            .Select(b => BuildBlockHouse(b, cells))
            .ToArray();

        Houses = Blocks.Concat(Rows).Concat(Columns).ToArray();
    }

    private static TestHouse BuildBlockHouse(int blockIndex, TestCell[,] cells)
    {
        var blockCells = new List<ICell>(9);
        int startRow = (blockIndex / 3) * 3;
        int startColumn = (blockIndex % 3) * 3;

        for (int dr = 0; dr < 3; dr++)
        {
            for (int dc = 0; dc < 3; dc++)
            {
                blockCells.Add(cells[startRow + dr, startColumn + dc]);
            }
        }

        return new TestHouse(blockCells.ToArray());
    }
}

public class TestCell : ICell
{
    public int Row { get; }
    public int Column { get; }
    public int Block { get; }
    public int? Number { get; private set; }
    public List<int> Candidate { get; }

    public TestCell(int row, int column)
    {
        Row = row;
        Column = column;
        Block = ((row - 1) / 3) * 3 + ((column - 1) / 3) + 1;
        Candidate = new List<int>();
    }

    public void SetNumber(int value)
    {
        Number = value;
        Candidate.Clear();
    }

    public void AddCandidates(params int[] candidates)
    {
        foreach (var digit in candidates)
        {
            if (!Candidate.Contains(digit))
            {
                Candidate.Add(digit);
            }
        }
    }
}
