namespace ProjectPingpong;

[JsonConverter(typeof(StringIdJsonConverter<MatchId>))]
public class MatchId : StringId
{
    public static readonly MatchId Default = new MatchId(string.Empty);
    public static class Types
    {
        public static readonly string LeagueMatch = "leaguematch";
        public static readonly string KnockoutMatch = "knockoutmatch";
        public static readonly string FreeMatch = "freematch";
    }

    private string _type = string.Empty;
    private CompetitionName _competitionName = CompetitionName.Default;

    public string Type
    {
        get
        {
            if (string.IsNullOrEmpty(_competitionName?.Id))
            {
                (_type, _competitionName) = Parse(Id);
            }
            return _type;
        }
        set
        {
            _type = value;
        }
    }
    public CompetitionName CompetitionName
    {
        get
        {
            if (string.IsNullOrEmpty(_competitionName?.Id))
            {
                (_type, _competitionName) = Parse(Id);
            }
            return _competitionName;
        }
        set
        {
            _competitionName = value;
        }
    }

    public MatchId() { }
    public MatchId(string id)
        : base(id)
    {
        (Type, CompetitionName) = Parse(id);
    }
    public MatchId(LeagueId leagueId, PlayerName playerName1, PlayerName playerName2)
        : this($"{Types.LeagueMatch}.{leagueId}.{playerName1}.{playerName2}")
    {
    }
    public MatchId(KnockoutId knockoutId, KnockoutDepth knockoutDepth, int index)
        : this($"{Types.KnockoutMatch}.{knockoutId}.{knockoutDepth}.{index}")
    {
    }

    private (string type, CompetitionName cName) Parse(string id)
    {
        if (id == string.Empty)
        {
            return (string.Empty, CompetitionName.Default);
        }
        else
        {
            var arr = id.Split('.');
            var type = arr[0];
            var cName = new CompetitionName(arr[1]);
            return (type, cName);
        }
    }

    public string ToUrl()
    {
        return Id.Replace(".", "__");
    }

    public static MatchId FromUrl(string matchId)
    {
        return new MatchId(matchId.Replace("__", "."));
    }
}
public class MatchData
{
    public MatchId Id { get; set; } = MatchId.Default;
    public Player? LeftPlayer { get; set; }
    public Player? RightPlayer { get; set; }
    public int LeftSetScore { get; set; } = default;
    public int RightSetScore { get; set; } = default;
    public List<SetData>? Sets { get; set; }
    public bool Finished { get; set; }

    [JsonIgnore] public IEnumerable<Player> PlayerList => new[] { LeftPlayer, RightPlayer }.Where(p => p != null).Select(p => p!);
    [JsonIgnore] public Player? Winner => 
        LeftSetScore == RightSetScore ? null :
        LeftSetScore > RightSetScore ? LeftPlayer :
                                        RightPlayer;
    [JsonIgnore] public Player? Loser => 
        LeftSetScore == RightSetScore ? null :
        LeftSetScore > RightSetScore ? RightPlayer :
                                        LeftPlayer;
    [JsonIgnore] public bool LeftWin => LeftPlayer == Winner;
    [JsonIgnore] public bool RightWin => RightPlayer == Winner;
    [JsonIgnore] public bool NotStarted => Sets?.Empty() ?? true;
    [JsonIgnore] public bool IsPlaying => !Finished && (Sets?.Any() ?? false);
    [JsonIgnore] public bool HasPlayingSet => !Finished && (Sets?.Any(set => set.IsPlaying) ?? false);
    [JsonIgnore] public bool WaitingNextSet => !Finished && IsPlaying && Sets.Empty(set => set.IsPlaying);

    public int MySetScore(Player player)
    {
        if (LeftPlayer?.Name == player.Name) return LeftSetScore;
        if (RightPlayer?.Name == player.Name) return RightSetScore;
        return 0;
    }
    public int MyGamePoint(Player? player)
    {
        var playingSet = Sets?.FirstOrDefault(set => set.IsPlaying);
        if (LeftPlayer == player)
        {
            return playingSet?.CurrentPoint?.LeftPoint ?? 0;
        }
        else if (RightPlayer == player)
        {
            return playingSet?.CurrentPoint?.RightPoint ?? 0;
        }
        else
        {
            return 0;
        }
    }
    public void StartNewSet()
    {
        Sets ??= new();
        Sets.Add(new SetData()
        {
            Status = SetStatus.Playing,
            History = new List<GamePoint>
            {
                new GamePoint(0, 0),
            }
        });
    }
    public void CancelSet()
    {
        var playingSet = Sets?.FirstOrDefault(set => set.IsPlaying);
        if (playingSet != null)
        {
            Sets!.Remove(playingSet);
        }
    }
    public void ConfirmSetResult()
    {
        var playingSet = Sets?.FirstOrDefault(set => set.Status == SetStatus.Playing);
        var finishData = CheckGameFinish();
        if (finishData.Finished && playingSet != null)
        {
            if (playingSet.CurrentPoint.LeftPoint > playingSet.CurrentPoint.RightPoint)
            {
                LeftSetScore++;
            }
            else
            {
                RightSetScore++;
            }
            playingSet.Status = SetStatus.End;
        }
    }
    public int? IncreaseLeftScore()
    {
        if (CheckGameFinish().Finished)
        {
            return null;
        }
        var playingSet = Sets?.FirstOrDefault(set => set.IsPlaying);
        return playingSet?.IncreaseLeft();
    }
    public int? IncreaseRightScore()
    {
        if (CheckGameFinish().Finished)
        {
            return null;
        }
        var playingSet = Sets?.FirstOrDefault(set => set.Status == SetStatus.Playing);
        return playingSet?.IncreaseRight();
    }
    public (bool Finished, Player? Winner, Player? Loser) CheckGameFinish()
    {
        var playingSet = Sets?.FirstOrDefault(set => set.Status == SetStatus.Playing);
        if (playingSet != null)
        {
            var leftPoint = playingSet.CurrentPoint.LeftPoint;
            var rightPoint = playingSet.CurrentPoint.RightPoint;

            if (leftPoint >= 11 || rightPoint >= 11)
            {
                if (Math.Abs(leftPoint - rightPoint) >= 2)
                {
                    if (leftPoint > rightPoint)
                    {
                        return (true, LeftPlayer, RightPlayer);
                    }
                    else
                    {
                        return (true, RightPlayer, LeftPlayer);
                    }
                }
            }
        }
        return (false, null, null);
    }
}
public class KnockoutMatchData : MatchData
{
    public KnockoutDepth Depth { get; set; } = KnockoutDepth.None;
    public int Index { get; set; } = default;
    public MatchId? LeftChildMatchId { get; set; }
    public MatchId? RightChildMatchId { get; set; }

    [JsonIgnore] public KnockoutMatchData? LeftMatch { get; set; }
    [JsonIgnore] public KnockoutMatchData? RightMatch { get; set; }
}
