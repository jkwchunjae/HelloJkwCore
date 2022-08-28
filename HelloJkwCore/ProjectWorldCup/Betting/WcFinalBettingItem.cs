namespace ProjectWorldCup;

public class WcFinalBettingItem<TTeam> : WcBettingItem<TTeam> where TTeam : Team
{
    public TTeam Pick0 => Picked.Skip(0).First();
    public TTeam Pick1 => Picked.Skip(1).First();
    public TTeam Pick2 => Picked.Skip(2).First();
    public TTeam Pick3 => Picked.Skip(3).First();
    public TTeam Fix0 => Fixed.Skip(0).First();
    public TTeam Fix1 => Fixed.Skip(1).First();
    public TTeam Fix2 => Fixed.Skip(2).First();
    public TTeam Fix3 => Fixed.Skip(3).First();
    /// <summary> 각 순위에 대한 점수 </summary>
    public int RankScore
    {
        get
        {
            var score = 0;

            if (Pick0 == Fix0) score += 32;
            if (Pick1 == Fix1) score += 8;
            if (Pick2 == Fix2) score += 4;
            if (Pick3 == Fix3) score += 2;

            return score;
        }
    }

    /// <summary> 결승 진출팀 점수 </summary>
    public int FinalMatchScore
    {
        get
        {
            var score = 0;

            if (Pick0 == Fix0 || Pick0 == Fix1) score += 5;
            if (Pick1 == Fix0 || Pick1 == Fix1) score += 5;

            return score;
        }
    }

    /// <summary> 4강 진출팀 점수 </summary>
    public int SemiFinalMatchScore
    {
        get
        {
            var score = 0;

            if (Fixed.Contains(Pick0)) score += 1;
            if (Fixed.Contains(Pick1)) score += 1;
            if (Fixed.Contains(Pick2)) score += 1;
            if (Fixed.Contains(Pick3)) score += 1;

            return score;
        }
    }

    /// <summary> 최종 점수 </summary>
    public override int Score
    {
        get => RankScore + FinalMatchScore + SemiFinalMatchScore;
        set { }
    }
}
