namespace ProjectWorldCup;

public class Team
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Flag { get; set; }
    public int FifaRank { get; set; }
    public string Region { get; set; }
}

public class TeamStanding<TTeam> where TTeam : Team
{
    public TTeam Team { get; set; }
    /// <summary> 승점 </summary>
    public int Point => Won * 3 + Drawn * 1 + Lost * 0;
    public int Rank { get; set; }
    /// <summary> 게임 수 </summary>
    public int Played => Won + Drawn + Lost;
    /// <summary> 승 </summary>
    public int Won { get; set; }
    /// <summary> 무 </summary>
    public int Drawn { get; set; }
    /// <summary> 패 </summary>
    public int Lost { get; set; }
    /// <summary> 득점 </summary>
    public int Gf { get; set; }
    /// <summary> 실점 </summary>
    public int Ga { get; set; }
    /// <summary> 골득실 </summary>
    public int Gd => Gf - Ga;
}