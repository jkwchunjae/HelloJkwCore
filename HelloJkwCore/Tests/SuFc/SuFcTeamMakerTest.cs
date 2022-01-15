using ProjectSuFc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests.SuFc;

public class SuFcTeamMakerTest
{
    private readonly List<MemberName> _names;

    public SuFcTeamMakerTest()
    {
        _names = new List<MemberName>
        {
            new MemberName("A"),
            new MemberName("B"),
            new MemberName("C"),
            new MemberName("D"),
            new MemberName("E"),

            new MemberName("F"),
            new MemberName("G"),
            new MemberName("H"),
            new MemberName("I"),
            new MemberName("J"),

            new MemberName("K"),
            new MemberName("L"),
        };
    }

    [Fact]
    public void SplitOptionTest_분리조건_사람수가_팀_수보다_적은경우()
    {
        var teamMaker = new SettingOptionTeamMaker();

        var teamCount = 3;
        var settingOption = new TeamSettingOption()
        {
            SplitOptions = new () { new () { Names = new () { _names[0], _names[1] }, }, },
        };

        var teamResult = teamMaker.MakeTeam_Internal(_names, teamCount, settingOption);

        var p0 = teamResult.Players.First(x => x.MemberName == _names[0]);
        var p1 = teamResult.Players.First(x => x.MemberName == _names[1]);

        var ordered = teamResult.GroupByTeam
            .OrderBy(x => x.Value.Count)
            .ToList();

        Assert.NotEqual(p0.TeamName.Id, p1.TeamName.Id);
        Assert.Equal(_names.Count, teamResult.Players.Count);
        Assert.Equal(teamCount, teamResult.GroupByTeam.Count);
        Assert.True(ordered.Last().Value.Count - ordered.First().Value.Count <= 1);
    }

    [Fact]
    public void SplitOptionTest_분리조건_사람수가_팀_수보다_많은경우()
    {
        var teamMaker = new SettingOptionTeamMaker();

        var teamCount = 3;
        var splitNames = new List<MemberName> { _names[2], _names[3], _names[4], _names[5] };
        var settingOption = new TeamSettingOption()
        {
            SplitOptions = new () { new () { Names = splitNames }, },
        };

        var result = teamMaker.MakeTeam_Internal(_names, teamCount, settingOption);

        var splitPlayers = result.Players.Where(x => splitNames.Any(name => name == x.MemberName));
        var grouped = splitPlayers
            .GroupBy(x => x.TeamName)
            .Select(x => new { TeamName = x.Key, MemberCount = x.Count() })
            .OrderBy(x => x.MemberCount)
            .ToList();

        Assert.Equal(1, grouped[0].MemberCount);
        Assert.Equal(1, grouped[1].MemberCount);
        Assert.Equal(2, grouped[2].MemberCount);
        Assert.Equal(_names.Count, result.Players.Count);
    }

    [Fact]
    public void SplitOptionTest_분리조건이_여러가지인_경우()
    {
        var teamMaker = new SettingOptionTeamMaker();

        var teamCount = 3;
        var splitNames = new List<MemberName> { _names[2], _names[3], _names[4], _names[5] };
        var settingOption = new TeamSettingOption()
        {
            SplitOptions = new() {
                new() { Names = new () { _names[0], _names[1] } },
                new() { Names = splitNames },
            },
        };

        var result = teamMaker.MakeTeam_Internal(_names, teamCount, settingOption);

        var team0 = result.Players.First(x => x.MemberName == _names[0]);
        var team1 = result.Players.First(x => x.MemberName == _names[1]);
        var splitTeams = result.Players
            .Where(x => splitNames.Any(name => name == x.MemberName))
            .GroupBy(x => x.TeamName)
            .Select(x => new { TeamName = x.Key, MemberCount = x.Count() })
            .OrderBy(x => x.MemberCount)
            .ToList();
        var ordered = result.GroupByTeam
            .OrderBy(x => x.Value.Count)
            .ToList();

        Assert.NotEqual(team0, team1);
        Assert.True(splitTeams.Last().MemberCount - splitTeams.First().MemberCount <= 1);
        Assert.True(ordered.Last().Value.Count - ordered.First().Value.Count <= 1);
    }
}