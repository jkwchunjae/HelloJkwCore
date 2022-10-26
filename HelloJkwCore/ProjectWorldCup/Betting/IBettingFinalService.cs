﻿using ProjectWorldCup.Pages;

namespace ProjectWorldCup;

public interface IBettingFinalService
{
    ValueTask<List<WcFinalBettingItem<Team>>> GetAllBettingsAsync();
    Task<WcFinalBettingItem<Team>> GetBettingAsync(BettingUser user);
    Task SaveBettingItemAsync(WcFinalBettingItem<Team> bettingItem);
    List<(string StageId, List<KnMatch> Matches)> EvaluateUserBetting(List<KnMatch> quarters, WcFinalBettingItem<Team> userBetting, List<KnMatch> matches);
    TeamButtonType GetButtonType(string stageId, Team team, List<(string StageId, List<KnMatch> Matches)> stageMatches, WcFinalBettingItem<Team> userBetting);
    List<(string StageId, List<KnMatch> Matches)> PickTeam(string stageId, string matchId, Team team, List<(string StageId, List<KnMatch> Matches)> stageMatches, List<KnMatch> matches);
    (List<(string StageId, List<KnMatch> Matches)> StageMatches, List<Team> PickTeams) PickRandom(List<(string StageId, List<KnMatch> Matches)> stageMatches, List<KnMatch> matches);
}
