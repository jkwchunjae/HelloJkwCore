<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load "./*.linq"
#load "./../*.linq"

void Main()
{
    TurbotFishExample();
}

void TurbotFishExample()
{
    var board = new BoardBuilder()
        .SetCandidate(1, 1, [5, 7])
        .SetCandidate(2, 2, [5, 8])
        .SetCandidate(4, 1, [1, 5])
        .SetCandidate(4, 8, [5, 9])
        .SetCandidate(2, 8, [3, 5])
        .Build();

    ExpectElimination(nameof(TurbotFishExample), new TurbotFishStrategy(), board, [(2, 8, 5)]);
}
