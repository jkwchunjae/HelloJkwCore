namespace ProjectWorldCup.FifaLibrary;

public class FifaMatchTeam
{

    [JsonProperty("teamId")]
    public string TeamId { get; set; }

    [JsonProperty("countryId")]
    public string CountryId { get; set; }

    [JsonProperty("confederationId")]
    public string ConfederationId { get; set; }

    [JsonProperty("teamName")]
    public string TeamName { get; set; }

    [JsonProperty("teamType")]
    public int TeamType { get; set; }

    [JsonProperty("gender")]
    public int Gender { get; set; }

    [JsonProperty("footballType")]
    public int FootballType { get; set; }

    [JsonProperty("abbreviation")]
    public string Abbreviation { get; set; }

    [JsonProperty("pictureUrl")]
    public string PictureUrl { get; set; }
    public int Score { get; set; }
}

public class FifaMatchData
{

    [JsonProperty("idCompetition")]
    public string IdCompetition { get; set; }

    [JsonProperty("idSeason")]
    public string IdSeason { get; set; }

    [JsonProperty("idStage")]
    public string IdStage { get; set; }

    [JsonProperty("idMatch")]
    public string IdMatch { get; set; }

    [JsonProperty("idGroup")]
    public string IdGroup { get; set; }

    [JsonProperty("groupName")]
    public string GroupName { get; set; }

    [JsonProperty("date")]
    public DateTime Date { get; set; }

    [JsonProperty("home")]
    public FifaMatchTeam Home { get; set; }

    [JsonProperty("away")]
    public FifaMatchTeam Away { get; set; }

    [JsonProperty("stadiumName")]
    public string StadiumName { get; set; }

    [JsonProperty("stadiumCityName")]
    public string StadiumCityName { get; set; }

    [JsonProperty("matchStatus")]
    public int MatchStatus { get; set; }

    [JsonProperty("resultType")]
    public int ResultType { get; set; }

    [JsonProperty("matchNumber")]
    public int MatchNumber { get; set; }

    [JsonProperty("competitionName")]
    public string CompetitionName { get; set; }

    [JsonProperty("matchDetailsUrl")]
    public string MatchDetailsUrl { get; set; }

    [JsonProperty("matchDetailsSectionUrl")]
    public string MatchDetailsSectionUrl { get; set; }
}

public class ActiveSeason
{

    [JsonProperty("seasonId")]
    public string SeasonId { get; set; }

    [JsonProperty("competitionId")]
    public string CompetitionId { get; set; }

    [JsonProperty("confederationIds")]
    public List<string> ConfederationIds { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("abbreviation")]
    public string Abbreviation { get; set; }

    [JsonProperty("startDate")]
    public DateTime StartDate { get; set; }

    [JsonProperty("endDate")]
    public DateTime EndDate { get; set; }

    [JsonProperty("pictureUrl")]
    public string PictureUrl { get; set; }

    [JsonProperty("mascotPictureUrl")]
    public string MascotPictureUrl { get; set; }

    [JsonProperty("matchBallPictureUrl")]
    public string MatchBallPictureUrl { get; set; }

    [JsonProperty("matches")]
    public List<FifaMatchData> Matches { get; set; }
}

public class Competition
{

    [JsonProperty("competitionId")]
    public string CompetitionId { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("gender")]
    public int Gender { get; set; }

    [JsonProperty("footballType")]
    public int FootballType { get; set; }

    [JsonProperty("competitionType")]
    public int CompetitionType { get; set; }

    [JsonProperty("competitionDetailsUrl")]
    public string CompetitionDetailsUrl { get; set; }

    [JsonProperty("activeSeasons")]
    public List<ActiveSeason> ActiveSeasons { get; set; }
}

public class MatchDataRoot
{

    [JsonProperty("competition")]
    public Competition Competition { get; set; }

    [JsonProperty("filtersLabel")]
    public string FiltersLabel { get; set; }

    [JsonProperty("noMatchesTodayLabel")]
    public string NoMatchesTodayLabel { get; set; }

    [JsonProperty("noMatchesThatDayLabel")]
    public string NoMatchesThatDayLabel { get; set; }

    [JsonProperty("seeAllHighlightsLabel")]
    public string SeeAllHighlightsLabel { get; set; }

    [JsonProperty("liveOnFifaLabel")]
    public string LiveOnFifaLabel { get; set; }

    [JsonProperty("changeDayLabel")]
    public string ChangeDayLabel { get; set; }

    [JsonProperty("matchDetailsLabel")]
    public string MatchDetailsLabel { get; set; }

    [JsonProperty("liveLabel")]
    public string LiveLabel { get; set; }

    [JsonProperty("todayLabel")]
    public string TodayLabel { get; set; }

    [JsonProperty("winsLabel")]
    public string WinsLabel { get; set; }

    [JsonProperty("onPenaltiesLabel")]
    public string OnPenaltiesLabel { get; set; }

    [JsonProperty("matchPostponedLabel")]
    public string MatchPostponedLabel { get; set; }

    [JsonProperty("postponedAbbreviationLabel")]
    public string PostponedAbbreviationLabel { get; set; }
}

