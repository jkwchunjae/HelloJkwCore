<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

public interface IBoard
{
    ICell[][] Grid { get; }
    IHouse[] Houses { get; }
}

public interface IHouse
{
    ICell[] Grid { get; }
    int[] RemainNumbers { get; }
    IEnumerable<ICell> FillCells => Grid.Where(x => x.Number.HasValue);
    IEnumerable<ICell> EmptyCells => Grid.Where(x => !x.Number.HasValue);
}

public interface ICell
{
    int? Number { get; }
    List<int> Candidate { get; }
}