namespace ProjectPingpong.Utils;

public interface ILeagueMatchGenerator
{
    IReadOnlyList<(Player Player1, Player Player2)> CreateLeagueMatch(List<Player> players);
}
public class LeagueMatchGenerator : ILeagueMatchGenerator
{
    // http://www.gogotak.com/bbs/board.php?bo_table=sub8_3&wr_id=1164

    public IReadOnlyList<(Player Player1, Player Player2)> CreateLeagueMatch(List<Player> players)
    {
        if (players.Empty())
        {
            return new List<(Player Player1, Player Player2)>();
        }
        var realPlayerCount = players.Count;
        var fixedPlayerCount = players.Count + players.Count % 2;
        var result = Enumerable.Range(0, fixedPlayerCount - 1)
            .SelectMany(i => GetSingleMatchSet(realPlayerCount, i))
            .Select(x => (players[x.Index1], players[x.Index2]))
            .ToList();

        return result;
    }

    public List<(int Index1, int Index2)> GetSingleMatchSet(int playerCount, int beginIndex)
    {
        // playerCount == 6
        //  0 5     0 1     0 2     0 3     0 4
        //  1 4     2 5     3 1     4 2     5 3
        //  2 3     3 4     4 5     5 1     1 2

        // playerCount == 5
        //  0 *     0 1     0 2     0 3     0 4
        //  1 4     2 *     3 1     4 2     * 3
        //  2 3     3 4     4 *     * 1     1 2

        var fixedCount = playerCount + playerCount % 2; // 짝수로 만듦

        var length = fixedCount / 2;
        var arr = new int[length][];
        var rows = Enumerable.Range(0, length)
            .Concat(Enumerable.Range(0, length).Select(x => length - x - 1))
            .ToList();

        arr[0] = new int[2] { 0, 0 };
        for (var i = 1; i < fixedCount; i++)
        {
            var row = rows[i];
            var column = 0;
            var playerIndex = (i - 1 + beginIndex) % (fixedCount - 1) + 1;
            if (i < length)
            {
                arr[row] = new int[2];
            }
            else
            {
                column = 1;
            }
            arr[row][column] = playerIndex;
        }

        return arr
            .Select(x => (x[0], x[1]))
            .Where(x => x.Item1 < playerCount && x.Item2 < playerCount)
            .ToList();
    }
}
