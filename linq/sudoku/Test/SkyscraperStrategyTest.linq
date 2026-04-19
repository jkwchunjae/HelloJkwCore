<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load "./*.linq"
#load "./../*.linq"

void Main()
{
    SkyscraperExample();
}

void SkyscraperExample()
{
    var board = new BoardBuilder()
        .SetCandidate(1, 2, [5, 7])
        .SetCandidate(5, 2, [5, 8])
        .SetCandidate(3, 8, [1, 5])
        .SetCandidate(5, 8, [5, 9])
        .SetCandidate(1, 7, [3, 5])
        .Build();

    ExpectElimination(nameof(SkyscraperExample), new SkyscraperStrategy(), board, [(1, 7, 5)]);
}
