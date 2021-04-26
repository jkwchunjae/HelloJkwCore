using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSuFc
{
    public partial class SuFcService : ISuFcService
    {
        IFileSystem _fs;
        private readonly Dictionary<TeamMakerStrategy, ITeamMaker> _strategy = new();

        public SuFcService(
            SuFcOption option,
            IFileSystemService fsService)
        {
            _fs = fsService.GetFileSystem(option.FileSystemSelect);
            _strategy.Add(TeamMakerStrategy.FullRandom, new FullRandomTeamMaker());
        }

        public Task<TeamResult> MakeTeamAsync(List<Member> members, int teamCount, TeamMakerStrategy strategy)
        {
            if (_strategy.ContainsKey(strategy))
            {
                var teamMaker = _strategy[strategy];
                var result = teamMaker.MakeTeam(members, teamCount);
                return Task.FromResult(result);
            }

            return Task.FromResult((TeamResult)null);
        }
    }
}
