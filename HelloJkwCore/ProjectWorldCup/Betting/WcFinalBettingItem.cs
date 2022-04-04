namespace ProjectWorldCup;

public class WcFinalBettingItem<TTeam> : WcBettingItem<TTeam> where TTeam : Team
{
    /// <summary> 각 순위에 대한 점수 </summary>
    public int RankScore
    {
        get
        {
            var score = 0;

            if (Picked[0] == Fixed[0]) score += 32;
            if (Picked[1] == Fixed[1]) score += 8;
            if (Picked[2] == Fixed[2]) score += 4;
            if (Picked[3] == Fixed[3]) score += 2;

            return score;
        }
    }

    /// <summary> 결승 진출팀 점수 </summary>
    public int FinalMatchScore
    {
        get
        {
            var score = 0;

            if (Picked[0] == Fixed[0] || Picked[0] == Fixed[1]) score += 5;
            if (Picked[1] == Fixed[0] || Picked[1] == Fixed[1]) score += 5;

            return score;
        }
    }

    /// <summary> 4강 진출팀 점수 </summary>
    public int SemiFinalMatchScore
    {
        get
        {
            var score = 0;

            if (Fixed.Contains(Picked[0])) score += 1;
            if (Fixed.Contains(Picked[1])) score += 1;
            if (Fixed.Contains(Picked[2])) score += 1;
            if (Fixed.Contains(Picked[3])) score += 1;

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
