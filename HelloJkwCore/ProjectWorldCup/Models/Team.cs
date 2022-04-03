﻿namespace ProjectWorldCup;

public class Team : IEquatable<Team>, IComparable<Team>
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Flag { get; set; }
    public int FifaRank { get; set; }
    public string Region { get; set; }

    public int CompareTo(Team other)
    {
        return Id.CompareTo(other.Id);
    }

    public bool Equals(Team other)
    {
        return Id == other?.Id;
    }

    public static bool operator ==(Team obj1, Team obj2)
    {
        if (ReferenceEquals(obj1, obj2))
        {
            return true;
        }
        if (ReferenceEquals(obj1, null))
        {
            return false;
        }
        if (ReferenceEquals(obj2, null))
        {
            return false;
        }

        return obj1.Equals(obj2);
    }

    public static bool operator !=(Team obj1, Team obj2)
    {
        return !(obj1 == obj2);
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as Team);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
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