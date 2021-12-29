using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProjectWorldCup
{
    public class QualifiedTeam
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public TeamFlag Flag { get; set; }
        public string Status { get; set; }
        public RankingTeamData Ranking { get; set; }

        public int Rank => Ranking?.RankingItem?.Rank ?? 0;
    }

    public partial class Fifa : IFifa
    {
        private List<QualifiedTeam> _qualifiedTeams = new();

        public async Task<List<QualifiedTeam>> GetQualifiedTeamsAsync()
        {
            try
            {
                var url = @"https://www.fifa.com/tournaments/mens/worldcup/qatar2022/qualifiers";

                var pageData = await GetPageData(new Uri(url));

                var list = pageData?["tournamentQualificationTeamsProps"]?["teams"]?.AsEnumerable() ?? null;

                var result = list?
                    .Select(x => x.ToObject<QualifiedTeam>())
                    .Where(x => x != null)
                    .Where(x => x?.Status == "Q")
                    .ToList();

                if (result?.Any() ?? false)
                {
                    _qualifiedTeams = result ?? new();
                }

                return result ?? new();
            }
            catch
            {
                if (_qualifiedTeams.Any())
                {
                    return _qualifiedTeams;
                }
                throw;
            }
        }
    }
}
