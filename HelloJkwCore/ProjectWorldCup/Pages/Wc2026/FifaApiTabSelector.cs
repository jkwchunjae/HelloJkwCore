namespace ProjectWorldCup.Pages.Wc2026;

public static class FifaApiTabSelector
{
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
}
