using ProjectWorldCup.Pages.Wc2026;

namespace Tests.WorldCup;

public class FifaApiTabSelectorTest
{
    [Fact]
    public void Selects_group_stage_before_round32_threshold()
    {
        var stages = CreateStages();
        var nowKst = new DateTime(2026, 6, 28, 3, 59, 0);

        var activeIndex = FifaApiTabSelector.GetActivePanelIndex(stages, nowKst);

        Assert.Equal(0, activeIndex);
    }

    [Fact]
    public void Selects_round32_one_day_before_first_round32_match()
    {
        var stages = CreateStages();
        var nowKst = new DateTime(2026, 6, 28, 4, 0, 0);

        var activeIndex = FifaApiTabSelector.GetActivePanelIndex(stages, nowKst);

        Assert.Equal(1, activeIndex);
    }

    [Fact]
    public void Selects_latest_stage_whose_threshold_has_passed()
    {
        var stages = CreateStages();
        var nowKst = new DateTime(2026, 7, 9, 5, 0, 0);

        var activeIndex = FifaApiTabSelector.GetActivePanelIndex(stages, nowKst);

        Assert.Equal(3, activeIndex);
    }

    private static IReadOnlyList<IReadOnlyList<FifaApiMatchRow>> CreateStages()
    {
        return new List<IReadOnlyList<FifaApiMatchRow>>
        {
            CreateRows(new DateTime(2026, 6, 12, 4, 0, 0)),
            CreateRows(new DateTime(2026, 6, 29, 4, 0, 0)),
            CreateRows(new DateTime(2026, 7, 5, 2, 0, 0)),
            CreateRows(new DateTime(2026, 7, 10, 5, 0, 0)),
        };
    }

    private static IReadOnlyList<FifaApiMatchRow> CreateRows(DateTime firstKickoffKst)
    {
        return new List<FifaApiMatchRow>
        {
            new FifaApiMatchRow
            {
                KickoffKst = firstKickoffKst,
            },
        };
    }
}
