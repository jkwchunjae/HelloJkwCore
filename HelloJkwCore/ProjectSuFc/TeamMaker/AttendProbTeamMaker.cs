using JkwExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSuFc
{
    public class AttendProbTeamMaker : TeamMaker
    {
        public override Task<TeamResult> MakeTeamAsync(List<MemberName> names, int teamCount, TeamSettingOption option)
        {
            //var members = await SuFcService.GetAllMember();

            //// 완전 랜덤으로 20개 뽑아 놓고 가장 좋은걸 골라보자.

            //var users = names
            //    .Select(name => new { Name = name, Member = members.FirstOrDefault(x => x.Name == name) })
            //    .Where(x => x.Member != null)
            //    .Select(x => new { x.Name, x.Member, AttendProp = x.Member.GetSpecValue(MemberSpecType.AttendProb) })
            //    .ToList();

            //var list = Enumerable.Range(0, 10)
            //    .Select(_ =>
            //    {
            //        var randomTeams = MakeRandomTeam(users.Count, teamCount);
            //        var teamedUsers = users
            //            .Zip(randomTeams, (x, t) => new { x.Name, x.Member, x.AttendProp, Team = t })
            //            .ToList();
            //        var avgByTeam = teamedUsers
            //            .GroupBy(x => x.Team)
            //            .Select(x => new { Team = x.Key, Avg = x.Sum(e => e.AttendProp) })
            //            .ToList();
            //        var sd = avgByTeam.Select(x => x.Avg).StandardDeviation();
            //        return new { Users = teamedUsers, AvgByTeam = avgByTeam, Score = sd };
            //    })
            //    .OrderBy(x => x.Score)
            //    .ToList();

            //// list 에서 만든 데이터 중에서 가장 좋은걸 고른다.
            //var best = list.First();

            //var teamResult = new TeamResult(teamCount);
            //teamResult.Score = best.Users
            //    .ToDictionary(x => x.Name, x => x.AttendProp);

            //teamResult.Players = best.Users
            //    .Select(x => (x.Name, x.Team))
            //    .ToList();

            //return teamResult;
            return Task.FromResult(new TeamResult());
        }
    }
}
