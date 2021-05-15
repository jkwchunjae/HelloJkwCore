using JkwExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSuFc
{
    public class SettingOptionTeamMaker : TeamMaker
    {
        public SettingOptionTeamMaker(ISuFcService service) : base(service)
        {
        }

        public override async Task<TeamResult> MakeTeamAsync(List<MemberName> names, int teamCount)
        {
            var teamSetting = await SuFcService.GetTeamSettingOption();
            return MakeTeam_Internal(names, teamCount, teamSetting);
        }

        public TeamResult MakeTeam_Internal(List<MemberName> names, int teamCount, TeamSettingOption teamSetting)
        {
            var teamResult = new TeamResult(teamCount);

            foreach (var splitOption in teamSetting.SplitOptions)
            {
                var remainNames = splitOption.Names.Where(x => teamResult.Players.Empty(e => e.MemberName == x)).ToList();
                var userCount = remainNames.Count; // count = 5, teamCount = 3
                var teamSetCount = userCount / teamCount + (userCount % teamCount == 0 ? 0 : 1); // teamSetCount = 2
                var teamNames = Enumerable.Range(1, teamSetCount)
                    .Select((_, i) => MakeTeamNameList(teamCount).Take(i == teamSetCount - 1 ? userCount % teamCount : teamCount))
                    .SelectMany(x => x)
                    .RandomShuffle()
                    .Take(userCount)
                    .ToList(); // ABCCB (random)

                var splitResult = remainNames.Zip(teamNames, (n, t) => (n, t)).ToList();

                teamResult.Players.AddRange(splitResult);
            }

            var remains = names
                .Where(x => teamResult.Players.Empty(e => e.MemberName == x))
                .RandomShuffle()
                .ToList();

            foreach (var name in remains)
            {
                var newTeam = GetNextTeamName(teamResult);

                teamResult.Players.Add((name, newTeam));
            }

            return teamResult;
        }

        private TeamName GetNextTeamName(TeamResult teamResult)
        {
            var ordered = teamResult.TeamNames
                .Select(x => new { TeamName = x, Count = teamResult.Players.Count(e => e.TeamName == x) })
                .OrderBy(x => x.Count)
                .ToList();

            if (ordered.First().Count == ordered.Last().Count)
            {
                // 모든 팀의 멤버 수가 다 같은 상태
                // 아무거나 골라 주자
                var result = ordered
                    .RandomShuffle()
                    .First()
                    .TeamName;
                return result;
            }
            else
            {
                var result = ordered.First().TeamName;
                return result;
            }
        }
    }
}
