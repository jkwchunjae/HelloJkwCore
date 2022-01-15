using ProjectWorldCup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests.WorldCup;

public class FifaGroupMatchesApiTest
{
    [Fact]
    public async Task FifaGroupStageMatches_should_return_48_matches()
    {
        IFifa fifa = new Fifa();

        var matches = await fifa.GetGroupStageMatchesAsync();

        Assert.Equal(48, matches.Count());
    }

    [Fact]
    public async Task FifaKnockoutStageMatches_should_return_16_matches()
    {
        IFifa fifa = new Fifa();

        var matches = await fifa.GetKnockoutStageMatchesAsync();

        Assert.Equal(16, matches.Count());
    }
}