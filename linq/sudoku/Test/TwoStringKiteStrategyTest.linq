<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load "./*.linq"
#load "./../*.linq"

void Main()
{
    TwoStringKiteExample();
}

void TwoStringKiteExample()
{
    var board = new BoardBuilder()
        .SetCandidate(8, 4, [2, 5])
        .SetCandidate(8, 9, [5, 9])
        .SetCandidate(2, 7, [1, 5])
        .SetCandidate(9, 7, [5, 6])
        .SetCandidate(2, 4, [4, 5])
        .Build();

    ExpectElimination(nameof(TwoStringKiteExample), new TwoStringKiteStrategy(), board, [(2, 4, 5)]);
}
