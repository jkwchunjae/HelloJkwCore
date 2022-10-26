using ProjectWorldCup.Pages;
using static MudBlazor.CategoryTypes;

namespace ProjectWorldCup;

public class BettingFinalService : IBettingFinalService
{
    private readonly IFileSystem _fs;
    private object _lock = new object();
    private List<WcFinalBettingItem<Team>> _cache = null;
    private IFifa _fifa;
    private IWorldCupService _worldCupService;
    private System.Timers.Timer _timer;

    public BettingFinalService(
        IFileSystemService fsService,
        IFifa fifa,
        IWorldCupService worldCupService,
        WorldCupOption option)
    {
        _fs = fsService?.GetFileSystem(option?.FileSystemSelect, option?.Path);
        _fifa = fifa;
        _worldCupService = worldCupService;

        //_timer = new System.Timers.Timer(TimeSpan.FromMinutes(10).TotalMilliseconds);
        //_timer.Elapsed += async (s, e) => await UpdateStandingsAsync();
        //_timer.AutoReset = true;
        //_timer.Start();
    }

    public async ValueTask<List<WcFinalBettingItem<Team>>> GetAllBettingsAsync()
    {
        lock (_lock)
        {
            if (_cache != null)
            {
                return _cache;
            }
        }
        var items = await _fs.ReadAllBettingItemsAsync<WcFinalBettingItem<Team>, Team>(BettingType.Final);
        lock (_lock)
        {
            _cache = items;
        }
        return items;
    }

    public async Task<WcFinalBettingItem<Team>> GetBettingAsync(BettingUser user)
    {
        lock (_lock)
        {
            if (_cache?.Any(x => x.User == user.AppUser) ?? false)
            {
                return _cache.First(x => x.User == user.AppUser);
            }
        }
        var bettingItem = await _fs.ReadBettingItemAsync<WcFinalBettingItem<Team>, Team>(BettingType.Final, user.AppUser);
        return bettingItem;
    }
    public async Task SaveTeamsAsync(BettingUser user, WcFinalBettingItem<Team> item)
    {
        await _fs.WriteBettingItemAsync(BettingType.Final, item);

        if (_cache?.Any() ?? false)
        {
            lock (_lock)
            {
                var index = _cache.FindIndex(x => x.User == item.User);
                if (index >= 0)
                {
                    _cache[index] = item;
                }
                else
                {
                    _cache.Add(item);
                }
            }
        }
    }

    #region Front
    public List<(string StageId, List<KnMatch> Matches)> EvaluateUserBetting(List<KnMatch> quarters, WcFinalBettingItem<Team> userBetting, List<KnMatch> matches)
    {
        if (userBetting?.Picked?.Count() == 4)
        {
            var semi = matches.Where(m => m.StageId == Fifa.Round4StageId).ToList();
            semi[0].HomeTeam = FindPickedTeam(quarters[1], userBetting.Picked);
            semi[0].AwayTeam = FindPickedTeam(quarters[0], userBetting.Picked);
            semi[1].HomeTeam = FindPickedTeam(quarters[3], userBetting.Picked);
            semi[1].AwayTeam = FindPickedTeam(quarters[2], userBetting.Picked);

            var third = matches.Where(m => m.StageId == Fifa.ThirdStageId).ToList();
            third[0].HomeTeam = FindPickedTeam(semi[0], new[] { userBetting.Pick2, userBetting.Pick3 });
            third[0].AwayTeam = FindPickedTeam(semi[1], new[] { userBetting.Pick2, userBetting.Pick3 });

            var final = matches.Where(m => m.StageId == Fifa.FinalStageId).ToList();
            final[0].HomeTeam = FindPickedTeam(semi[0], new[] { userBetting.Pick0, userBetting.Pick1 });
            final[0].AwayTeam = FindPickedTeam(semi[1], new[] { userBetting.Pick0, userBetting.Pick1 });

            return new List<(string StageId, List<KnMatch> Matches)>
            {
                (Fifa.Round8StageId, quarters),
                (Fifa.Round4StageId, semi),
                (Fifa.ThirdStageId, third),
                (Fifa.FinalStageId, final),
            };
        }
        else
        {
            return new List<(string StageId, List<KnMatch> Matches)>
            {
                (Fifa.Round8StageId, quarters),
            };
        }

        Team FindPickedTeam(KnMatch match, IEnumerable<Team> userPick)
        {
            if (userPick.Any(picked => picked == match.HomeTeam))
            {
                return match.HomeTeam;
            }
            if (userPick.Any(picked => picked == match.AwayTeam))
            {
                return match.AwayTeam;
            }
            return null;
        }
    }

    public TeamButtonType GetButtonType(string stageId, Team team, List<(string StageId, List<KnMatch> Matches)> stageMatches, WcFinalBettingItem<Team> userBetting)
    {
        if (team == null)
        {
            return TeamButtonType.Pickable;
        }
        if (stageId == Fifa.Round8StageId)
        {
            var semi = stageMatches.FirstOrDefault(stageMatches => stageMatches.StageId == Fifa.Round4StageId);
            if (semi.Matches?.Any(m => m?.HomeTeam == team || m?.AwayTeam == team) ?? false)
            {
                return TeamButtonType.Picked;
            }
            else
            {
                return TeamButtonType.Pickable;
            }
        }
        else if (stageId == Fifa.Round4StageId)
        {
            var final = stageMatches.FirstOrDefault(matches => matches.StageId == Fifa.FinalStageId);
            if (final.Matches?.Any(m => m?.HomeTeam == team || m?.AwayTeam == team) ?? false)
            {
                return TeamButtonType.Picked;
            }
            else
            {
                return TeamButtonType.Pickable;
            }
        }
        else if (stageId == Fifa.ThirdStageId)
        {
            if (userBetting?.Pick2 == team)
            {
                return TeamButtonType.Picked;
            }
            else
            {
                return TeamButtonType.Pickable;
            }
        }
        else
        {
            if (userBetting?.Pick0 == team)
            {
                return TeamButtonType.Picked;
            }
            else
            {
                return TeamButtonType.Pickable;
            }
        }
    }

    public List<(string StageId, List<KnMatch> Matches)> PickTeam(string stageId, string matchId, Team team, List<(string StageId, List<KnMatch> Matches)> stageMatches, List<KnMatch> matches)
    {
        matches = matches.Select(m => new KnMatch(m)).ToList();
        if (stageId == Fifa.Round8StageId)
        {
            #region 8강
            if (stageMatches.Empty(s => s.StageId == Fifa.Round4StageId))
            {
                stageMatches.Add((Fifa.Round4StageId, new()));
            }
            var quarter = stageMatches.First(s => s.StageId == Fifa.Round8StageId).Matches;
            var matchIndex = quarter.FindIndex(m => m.MatchId == matchId);
            var semi = stageMatches.First(s => s.StageId == Fifa.Round4StageId).Matches;

            if (matchIndex == 0 || matchIndex == 1)
            {
                if (semi.Empty())
                    semi.Add(matches.First(m => m.StageId == Fifa.Round4StageId));
                else if (semi[0] == null)
                    semi[0] = matches.First(m => m.StageId == Fifa.Round4StageId);
            }
            else if (matchIndex == 2 || matchIndex == 3)
            {
                if (semi.Empty())
                {
                    semi.Add(null);
                }
                if (semi.Count == 1)
                {
                    semi.Add(matches.Where(m => m.StageId == Fifa.Round4StageId).ToList()[1]);
                }
                else if (semi.Count == 2)
                {
                    //semi[1] = new KnMatch(matches.Where(m => m.StageId == Fifa.Round4StageId).ToList()[1]);
                }
            }

            Team prevTeam = null;
            if (matchIndex == 0)
            {
                prevTeam = semi[0].AwayTeam;
                semi[0].AwayTeam = team;
            }
            else if (matchIndex == 1)
            {
                prevTeam = semi[0].HomeTeam;
                semi[0].HomeTeam = team;
            }
            else if (matchIndex == 2)
            {
                prevTeam = semi[1].AwayTeam;
                semi[1].AwayTeam = team;
            }
            else if (matchIndex == 3)
            {
                prevTeam = semi[1].HomeTeam;
                semi[1].HomeTeam = team;
            }

            if (prevTeam != null)
            {
                if (stageMatches.Any(s => s.StageId == Fifa.ThirdStageId))
                {
                    var third = stageMatches.First(s => s.StageId == Fifa.ThirdStageId).Matches;
                    if (third.Any())
                    {
                        third[0].HomeTeam = null;
                        third[0].AwayTeam = null;
                    }
                }
                if (stageMatches.Any(s => s.StageId == Fifa.FinalStageId))
                {
                    var final = stageMatches.First(s => s.StageId == Fifa.FinalStageId).Matches;
                    if (final.Any())
                    {
                        final[0].HomeTeam = null;
                        final[0].AwayTeam = null;
                    }
                }
            }
            #endregion
        }
        else if (stageId == Fifa.Round4StageId)
        {
            #region 4강
            if (stageMatches.Empty(s => s.StageId == Fifa.ThirdStageId))
            {
                stageMatches.Add((Fifa.ThirdStageId, matches.Where(m => m.StageId == Fifa.ThirdStageId).ToList()));
            }
            if (stageMatches.Empty(s => s.StageId == Fifa.FinalStageId))
            {
                stageMatches.Add((Fifa.FinalStageId, matches.Where(m => m.StageId == Fifa.FinalStageId).ToList()));
            }

            var semi = stageMatches.First(s => s.StageId == Fifa.Round4StageId).Matches;
            var matchIndex = semi.FindIndex(m => m?.MatchId == matchId);
            var loserTeam = semi[matchIndex].HomeTeam == team ? semi[matchIndex].AwayTeam : semi[matchIndex].HomeTeam;

            var thirdMatch = stageMatches.First(s => s.StageId == Fifa.ThirdStageId).Matches;
            var finalMatch = stageMatches.First(s => s.StageId == Fifa.FinalStageId).Matches;

            if (matchIndex == 0)
            {
                thirdMatch[0].HomeTeam = loserTeam;
                finalMatch[0].HomeTeam = team;
            }
            else // if (matchIndex == 1)
            {
                thirdMatch[0].AwayTeam = loserTeam;
                finalMatch[0].AwayTeam = team;
            }
            #endregion
        }
        return stageMatches;
    }

    public (List<(string StageId, List<KnMatch> Matches)> StageMatches, List<Team> PickTeams) PickRandom(List<(string StageId, List<KnMatch> Matches)> stageMatches, List<KnMatch> matches)
    {
        var round8Matches = stageMatches.First(x => x.StageId == Fifa.Round8StageId).Matches;
        foreach (var match in round8Matches)
        {
            var pickTeam = StaticRandom.Next() < 0.5 ? match.HomeTeam : match.AwayTeam;
            stageMatches = PickTeam(match.StageId, match.MatchId, pickTeam, stageMatches, matches);
        }

        var round4Matches = stageMatches.First(x => x.StageId == Fifa.Round4StageId).Matches;
        foreach (var match in round4Matches)
        {
            var pickTeam = StaticRandom.Next() < 0.5 ? match.HomeTeam : match.AwayTeam;
            stageMatches = PickTeam(match.StageId, match.MatchId, pickTeam, stageMatches, matches);
        }

        var thirdMatch = stageMatches.First(x => x.StageId == Fifa.ThirdStageId).Matches[0];
        var finalMatch = stageMatches.First(x => x.StageId == Fifa.FinalStageId).Matches[0];

        var thirdResult = (new[] { thirdMatch.HomeTeam, thirdMatch.AwayTeam }).RandomShuffle();
        var finalResult = (new[] { finalMatch.HomeTeam, finalMatch.AwayTeam }).RandomShuffle();

        var pickTeams = finalResult.Concat(thirdResult).ToList();
        return (stageMatches, pickTeams);
    }
    #endregion
}
