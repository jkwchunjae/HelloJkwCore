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

    [Fact]
    public void Initial_scroll_target_selects_first_match_today()
    {
        var stages = new List<IReadOnlyList<FifaApiMatchRow>>
        {
            new List<FifaApiMatchRow>
            {
                Row(new DateTime(2026, 6, 14, 23, 0, 0)),
                Row(new DateTime(2026, 6, 15, 7, 0, 0)),
                Row(new DateTime(2026, 6, 15, 11, 0, 0)),
                Row(new DateTime(2026, 6, 16, 4, 0, 0)),
            },
        };

        var target = FifaApiTabSelector.GetInitialScrollTarget(stages, new DateTime(2026, 6, 15, 20, 0, 0));

        Assert.True(target.HasValue);
        var actual = target.GetValueOrDefault();
        Assert.Equal(0, actual.StageIndex);
        Assert.Equal(1, actual.RowIndex);
    }

    [Fact]
    public void Initial_scroll_target_falls_back_to_next_match_when_today_has_no_matches()
    {
        var stages = new List<IReadOnlyList<FifaApiMatchRow>>
        {
            new List<FifaApiMatchRow>
            {
                Row(new DateTime(2026, 6, 14, 23, 0, 0)),
            },
            new List<FifaApiMatchRow>
            {
                Row(new DateTime(2026, 6, 16, 4, 0, 0)),
            },
        };

        var target = FifaApiTabSelector.GetInitialScrollTarget(stages, new DateTime(2026, 6, 15, 20, 0, 0));

        Assert.True(target.HasValue);
        var actual = target.GetValueOrDefault();
        Assert.Equal(1, actual.StageIndex);
        Assert.Equal(0, actual.RowIndex);
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
            Row(firstKickoffKst),
        };
    }

    private static FifaApiMatchRow Row(DateTime kickoffKst)
    {
        return new FifaApiMatchRow
        {
            KickoffKst = kickoffKst,
        };
    }
}
