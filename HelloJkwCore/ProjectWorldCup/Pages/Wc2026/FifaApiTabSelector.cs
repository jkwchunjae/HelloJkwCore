namespace ProjectWorldCup.Pages.Wc2026;

public static class FifaApiTabSelector
{
    public static FifaApiScrollTarget? GetInitialScrollTarget(IReadOnlyList<IReadOnlyList<FifaApiMatchRow>> stageRows, DateTime nowKst)
    {
        var today = nowKst.Date;
        var todayMatch = FindFirst(stageRows, row => row.KickoffKst.Date == today);
        if (todayMatch != null)
        {
            return todayMatch;
        }

        return FindFirst(stageRows, row => row.KickoffKst >= nowKst);
    }

    public static int GetActivePanelIndex(IReadOnlyList<IReadOnlyList<FifaApiMatchRow>> stageRows, DateTime nowKst)
    {
        var activePanelIndex = 0;

        for (var i = 0; i < stageRows.Count; i++)
        {
            var firstKickoffKst = stageRows[i]
                .OrderBy(row => row.KickoffKst)
                .FirstOrDefault()
                ?.KickoffKst;

            if (firstKickoffKst == null)
            {
                continue;
            }

            if (nowKst >= firstKickoffKst.Value.AddDays(-1))
            {
                activePanelIndex = i;
            }
        }

        return activePanelIndex;
    }

    private static FifaApiScrollTarget? FindFirst(
        IReadOnlyList<IReadOnlyList<FifaApiMatchRow>> stageRows,
        Func<FifaApiMatchRow, bool> predicate)
    {
        return stageRows
            .SelectMany((rows, stageIndex) => rows
                .Select((row, rowIndex) => new
                {
                    Target = new FifaApiScrollTarget(stageIndex, rowIndex),
                    Row = row,
                }))
            .Where(match => predicate(match.Row))
            .OrderBy(match => match.Row.KickoffKst)
            .Select(match => (FifaApiScrollTarget?)match.Target)
            .FirstOrDefault();
    }
}

public readonly record struct FifaApiScrollTarget(int StageIndex, int RowIndex);
