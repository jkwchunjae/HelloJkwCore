using ProjectWorldCup.FifaLibrary;

namespace Tests.WorldCup;

public class FifaMatchDataTest
{
    [Fact]
    public void MakeHomeTeamData_from_StandingData()
    {
        var fifaMatchData = new FifaMatchData
        {
            Home = null,
            PlaceHolderA = "2C",
            PlaceHolderB = "1D",
        };
        var standing = new List<FifaStandingData>
        {
            new FifaStandingData
            {
                Group = new List<FifaIdName> { new FifaIdName { Locale = "en-GB", Description = "Group C" } },
                Position = 2,
                Played = 3,
                Team = new FifaStandingTeam
                {
                    IdTeam = "id-team",
                    IdCountry = "id-country",
                    PictureUrl = "pic",
                    Name = new List<FifaIdName> { new FifaIdName { Locale = "", Description = "name" } },
                }
            }
        };

        var match = KnMatch.CreateFromFifaMatchData(fifaMatchData, standing);
        Assert.NotNull(match);
        Assert.Equal("name", match.HomeTeam.Name);
        Assert.Equal("pic", match.HomeTeam.Flag);
        Assert.Equal("id-country", match.HomeTeam.Id);
    }
}
