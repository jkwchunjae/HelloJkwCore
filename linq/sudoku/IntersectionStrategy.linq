<Query Kind="Program">
  <Namespace>System.Diagnostics.CodeAnalysis</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load ".\SudokuModel"
#load ".\IStrategy"

public class IntersectionRowColumnStrategy : ICandidateStrategy
{
    public string Name => "IntersectionRowColumn";
    public string Description => "";

    public bool TryRemoveCandidate(IBoard board, [NotNullWhen(true)] out IEnumerable<StrategyResult>? result)
    {
        Func<IBoard, IHouse[]> getRows = board => board.Rows;
        Func<IBoard, IHouse[]> getColumns = board => board.Columns;

        if (TryRemoveCandidateInternal(board, getRows, out result))
        {
            return true;
        }
        if (TryRemoveCandidateInternal(board, getColumns, out result))
        {
            return true;
        }
        result = null;
        return false;
    }
    private bool TryRemoveCandidateInternal(IBoard board, Func<IBoard, IHouse[]> getHouses, [NotNullWhen(true)] out IEnumerable<StrategyResult>? result)
    {
        foreach (var house in getHouses(board))
        {
            foreach (var candidate in house.RemainNumbers)
            {
                var cells = house.FilterByCandidate(candidate);
                if (TryGetIntersectionHouse(board, cells, out var intersectionHouse))
                {
                    var otherCells = intersectionHouse.FilterByCandidate(candidate);

                    if (cells.Count() < otherCells.Count())
                    {
                        // 지울만한 뭔가가 있다.
                        result = otherCells.Except(cells)
                            .Select(cell => new StrategyResult
                            {
                                Target = cell,
                                Value = candidate,
                            });
                        return true;
                    }
                }
            }
        }

        result = null;
        return false;
    }

    bool TryGetIntersectionHouse(IBoard board, IEnumerable<ICell> cells, [NotNullWhen(true)] out IHouse? house)
    {
        if (cells.Count() >= 2)
        {
            if (cells.All(cell => cell.Block == cells.First().Block))
            {
                house = board.GetBlock(cells.First().Block);
                return true;
            }
        }

        house = null;
        return false;
    }
}

public class IntersectionBlockStrategy : ICandidateStrategy
{
    public string Name => "IntersectionBlock";
    public string Description => "";

    public bool TryRemoveCandidate(IBoard board, [NotNullWhen(true)] out IEnumerable<StrategyResult>? result)
    {
        foreach (var block in board.Blocks)
        {
            foreach (var candidate in block.RemainNumbers)
            {
                var cells = block.FilterByCandidate(candidate);
                if (TryGetIntersectionHouse(board, cells, out var intersectionHouse))
                {
                    // 블럭 내에서 특정 숫자가 모두 한 행에 있다면,
                    // 그 행의 다른 블럭에 있는 숫자는 후보에서 삭제한다.
                    var otherCells = intersectionHouse.FilterByCandidate(candidate);

                    if (cells.Count() < otherCells.Count())
                    {
                        // 지울만한 뭔가가 있다.
                        result = otherCells.Except(cells)
                            .Select(cell => new StrategyResult
                            {
                                Target = cell,
                                Value = candidate,
                            });
                        return true;
                    }
                }
            }
        }

        result = null;
        return false;
    }

    bool TryGetIntersectionHouse(IBoard board, IEnumerable<ICell> cells, [NotNullWhen(true)] out IHouse? house)
    {
        if (cells.Count() >= 2)
        {
            if (cells.All(cell => cell.Row == cells.First().Row))
            {
                house = board.GetRow(cells.First().Row);
                return true;
            }
            else if (cells.All(cell => cell.Column == cells.First().Column))
            {
                house = board.GetColumn(cells.First().Column);
                return true;
            }
        }

        house = null;
        return false;
    }
}