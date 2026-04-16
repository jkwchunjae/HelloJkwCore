<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load "./*.linq"
#load "./../*.linq"

void Main()
{
    NakedPairLeft();
    NakedPairRight();
    NakedTripleLeft();
    NakedTripleRight();
    NakedQuadrupleLeft();
    NakedQuadrupleRight();
}

void NakedPairLeft()
{
    ICell[] cells =
    [
        Cell.Filled(1),
        Cell.Empty(3, 7),
        Cell.Empty(3, 9),
        Cell.Empty(3, 9),
        Cell.Empty(2, 7),
        Cell.Filled(8),
        Cell.Empty(2,4,5,7),
        Cell.Filled(6),
        Cell.Empty(2,4,5,7),
    ];
    IHouse house = new TestHouse(cells);
    IBoard board = new SingleHouseBoard(house);
    
    var nakedSubsetStrategy = new NakedSubsetStrategy();
    if (nakedSubsetStrategy.TryRemoveCandidate(board, out var result))
    {
        result.Dump(nameof(NakedPairLeft), 1);
    }
    else
    {
        "nothing".Dump();
    }
}

void NakedPairRight()
{
    ICell[] cells =
    [
        Cell.Empty(5,8,9),
        Cell.Empty(8, 9),
        Cell.Filled(7),
        Cell.Empty(1,2,4,8,9),
        Cell.Empty(1,2,4,8,9),
        Cell.Empty(8,9),
        Cell.Empty(4,5,6,8,9),
        Cell.Empty(3,4,6,8,9),
        Cell.Empty(3,5,8,9),
    ];
    IHouse house = new TestHouse(cells);
    IBoard board = new SingleHouseBoard(house);

    var nakedSubsetStrategy = new NakedSubsetStrategy();
    if (nakedSubsetStrategy.TryRemoveCandidate(board, out var result))
    {
        result.Dump(nameof(NakedPairRight), 1);
    }
    else
    {
        "nothing".Dump();
    }
}

void NakedTripleLeft()
{
    ICell[] cells =
    [
        Cell.Empty(1,6,7),
        Cell.Empty(3,9),
        Cell.Filled(8),
        Cell.Empty(6, 9),
        Cell.Empty(3,6,9),
        Cell.Empty(1,7),
        Cell.Filled(5),
        Cell.Filled(2),
        Cell.Filled(4),
    ];
    IHouse house = new TestHouse(cells);
    IBoard board = new SingleHouseBoard(house);

    var nakedSubsetStrategy = new NakedSubsetStrategy();
    if (nakedSubsetStrategy.TryRemoveCandidate(board, out var result))
    {
        result.Dump(nameof(NakedTripleLeft), 1);
    }
    else
    {
        "nothing".Dump();
    }
}

void NakedTripleRight()
{
    ICell[] cells =
    [
        Cell.Empty(1,2,4,5,6),
        Cell.Empty(1,2,6),
        Cell.Empty(1,2,5,6,8),
        Cell.Empty(1,2,4,7),
        Cell.Empty(1,2,3,7,9),
        Cell.Empty(1,2,3,7,8,9),
        Cell.Empty(1,2,6),
        Cell.Empty(1,2,6),
        Cell.Empty(1,2,6,8),
    ];
    IHouse house = new TestHouse(cells);
    IBoard board = new SingleHouseBoard(house);

    var nakedSubsetStrategy = new NakedSubsetStrategy();
    if (nakedSubsetStrategy.TryRemoveCandidate(board, out var result))
    {
        result.Dump(nameof(NakedTripleRight), 1);
    }
    else
    {
        "nothing".Dump();
    }
}

void NakedQuadrupleLeft()
{
    ICell[] cells =
    [
        Cell.Empty(3,4,8,9),
        Cell.Empty(2,6,7),
        Cell.Empty(4,8,9),
        Cell.Empty(4,9),
        Cell.Empty(5,6),
        Cell.Empty(3,9),
        Cell.Empty(3,6,7,8),
        Cell.Empty(2,5,7,9),
        Cell.Filled(1),
    ];
    IHouse house = new TestHouse(cells);
    IBoard board = new SingleHouseBoard(house);

    var nakedSubsetStrategy = new NakedSubsetStrategy();
    if (nakedSubsetStrategy.TryRemoveCandidate(board, out var result))
    {
        result.Dump(nameof(NakedQuadrupleLeft), 1);
    }
    else
    {
        "nothing".Dump();
    }
}

void NakedQuadrupleRight()
{
    ICell[] cells =
    [
        Cell.Empty(2,3,4,6,8),
        Cell.Empty(4,5,6,8,9),
        Cell.Empty(4,6,7,9),
        Cell.Empty(2,3,4),
        Cell.Empty(4,9),
        Cell.Empty(4,7,9),
        Cell.Empty(1,2,4,6),
        Cell.Empty(1,4,5,6),
        Cell.Empty(4,6,7),
    ];
    IHouse house = new TestHouse(cells);
    IBoard board = new SingleHouseBoard(house);

    var nakedSubsetStrategy = new NakedSubsetStrategy();
    if (nakedSubsetStrategy.TryRemoveCandidate(board, out var result))
    {
        result.Dump(nameof(NakedQuadrupleRight), 1);
    }
    else
    {
        "nothing".Dump();
    }
}