<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load "./*.linq"
#load "./../*.linq"

void Main()
{
    XWingRowExample2();
    XWingRowExample3();
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
