namespace ProjectWorldCup.FifaLibrary;

public enum Gender
{
    Men,
    Women,
}

public class OverviewTeam
{
    [JsonProperty("groupPlacement")]
    public string Placement { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("flag")]
    public TeamFlag Flag { get; set; }
}

public class OverviewGroup
{
    [JsonProperty("groupTitle")]
    public string GroupName { get; set; }
    [JsonProperty("teams")]
    public List<OverviewTeam> Teams { get; set; }

    [JsonIgnore]
    public string Name => GroupName;
}

public class OverviewGroupDataRoot
{
    [JsonProperty("groups")]
    public List<OverviewGroup> Groups { get; set; }
}


public class TeamFlag
{
    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("src")]
    public string Src { get; set; }
    [JsonProperty("alt")]
    public string Alt { get; set; }
}

public class TeamTag
{
    public string Id { get; set; }
    public string Text { get; set; }
}

public class FifaStandingData
{

    [JsonProperty("matchDay")]
    public int MatchDay { get; set; }

    [JsonProperty("idCompetition")]
    public string IdCompetition { get; set; }

    [JsonProperty("idSeason")]
    public string IdSeason { get; set; }

    [JsonProperty("idStage")]
    public string IdStage { get; set; }

    [JsonProperty("idTeam")]
    public string IdTeam { get; set; }

    [JsonProperty("idGroup")]
    public string IdGroup { get; set; }

    [JsonProperty("groupName")]
    public string GroupName { get; set; }

    [JsonProperty("date")]
    public DateTime Date { get; set; }

    [JsonProperty("won")]
    public int Won { get; set; }

    [JsonProperty("lost")]
    public int Lost { get; set; }

    [JsonProperty("drawn")]
    public int Drawn { get; set; }

    [JsonProperty("played")]
    public int Played { get; set; }

    [JsonProperty("against")]
    public int Against { get; set; }

    [JsonProperty("for")]
    public int For { get; set; }

    [JsonProperty("position")]
    public int Position { get; set; }

    [JsonProperty("points")]
    public int Points { get; set; }

    [JsonProperty("goalsDifference")]
    public int GoalsDifference { get; set; }

    [JsonProperty("teamName")]
    public string TeamName { get; set; }

    [JsonProperty("teamLogo")]
    public string TeamLogo { get; set; }

    [JsonProperty("startDate")]
    public DateTime StartDate { get; set; }

    [JsonProperty("endDate")]
    public DateTime EndDate { get; set; }
}

public class FifaStanding
{

    [JsonProperty("standing")]
    public IList<FifaStandingData> Standing { get; set; }

    [JsonProperty("competitionName")]
    public string CompetitionName { get; set; }
}

public class FifaStandingDataRoot
{

    [JsonProperty("standings")]
    public IList<FifaStanding> Standings { get; set; }

    [JsonProperty("latestNewsLabel")]
    public string LatestNewsLabel { get; set; }

    [JsonProperty("changeDayLabel")]
    public string ChangeDayLabel { get; set; }

    [JsonProperty("teamLabel")]
    public string TeamLabel { get; set; }

    [JsonProperty("playedAbbreviationLabel")]
    public string PlayedAbbreviationLabel { get; set; }

    [JsonProperty("winsAbbreviationLabel")]
    public string WinsAbbreviationLabel { get; set; }

    [JsonProperty("drawsAbbreviationLabel")]
    public string DrawsAbbreviationLabel { get; set; }

    [JsonProperty("lossesAbbreviationLabel")]
    public string LossesAbbreviationLabel { get; set; }

    [JsonProperty("goalsForAbbreviationLabel")]
    public string GoalsForAbbreviationLabel { get; set; }

    [JsonProperty("goalsAgainstAbbreviationLabel")]
    public string GoalsAgainstAbbreviationLabel { get; set; }

    [JsonProperty("goalsDifferenceAbbreviationLabel")]
    public string GoalsDifferenceAbbreviationLabel { get; set; }

    [JsonProperty("pointsAbbreviationLabel")]
    public string PointsAbbreviationLabel { get; set; }
}

