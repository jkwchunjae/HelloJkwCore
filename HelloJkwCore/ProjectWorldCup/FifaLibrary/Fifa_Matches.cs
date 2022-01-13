using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectWorldCup
{
    public partial class Fifa : IFifa
    {
        private readonly string GroupStageMatchesCacheKey = nameof(GroupStageMatchesCacheKey);
        private readonly string KnockoutStageMatchesCacheKey = nameof(KnockoutStageMatchesCacheKey);

        public async Task<List<FifaMatchData>> GetGroupStageMatchesAsync()
        {
            return await GetFromCacheOrAsync<List<FifaMatchData>>(GroupStageMatchesCacheKey, async () =>
            {
                var url = "https://www.fifa.com/tournaments/mens/worldcup/qatar2022";
                var pageData = await GetPageData(new Uri(url));

                var groupStageMatches = pageData?["content"]?.ToArray()?[4]?["groupPhaseMatches"]
                    ?.SelectMany(x => x?["matches"]?.Select(e => e?.ToObject<FifaMatchData>())?.ToList() ?? new List<FifaMatchData>())
                    .ToList();

                return groupStageMatches ?? new();
            });
        }

        public async Task<List<FifaMatchData>> GetKnockoutStageMatchesAsync()
        {
            return await GetFromCacheOrAsync<List<FifaMatchData>>(KnockoutStageMatchesCacheKey, async () =>
            {
                var url = "https://www.fifa.com/tournaments/mens/worldcup/qatar2022";
                var pageData = await GetPageData(new Uri(url));

                var knockoutStageMatches = pageData?["content"]?.ToArray()?[4]?["knockoutPhaseMatches"]
                    ?.SelectMany(x => x?["matches"]?.Select(e => e?.ToObject<FifaMatchData>())?.ToList() ?? new List<FifaMatchData>())
                    .ToList();

                return knockoutStageMatches ?? new();
            });
        }
    }
}
