<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

public interface IBoard
{
    ICell[][] Grid { get; }
    IHouse[] Houses { get; } // => Blocks.Concat(Rows).Concat(Columns).ToArray();
    IHouse[] Blocks { get; }
    IHouse[] Rows { get; }
    IHouse[] Columns { get; }
    
    IHouse GetBlock(int block) => Blocks[block - 1];
    IHouse GetRow(int row) => Rows[row - 1];
    IHouse GetColumn(int column) => Columns[column - 1];
}

public interface IHouse
{
    ICell[] Cells { get; }
    int[] RemainNumbers { get; }
    IEnumerable<ICell> FillCells => Cells.Where(x => x.Number.HasValue);
    IEnumerable<ICell> EmptyCells => Cells.Where(x => !x.Number.HasValue);
    
    IEnumerable<ICell> FilterByCandidate(int candidate)
    {
        return Cells.Where(cell => cell.Candidate.Contains(candidate));
    }
}

public interface ICell
{
    int Row { get; }
    int Column { get; }
    int Block { get; }
    int? Number { get; }
    List<int> Candidate { get; }
}