using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using JkwExtensions;

namespace ProjectSuFc
{
    public partial class SuFcService : ISuFcService
    {
        private readonly Dictionary<TeamMakerStrategy, ITeamMaker> _strategy = new();
        private List<TeamResult> _teamResultList = null;

        private void InitTeamMaker()
        {
            _strategy.Add(TeamMakerStrategy.FullRandom, new FullRandomTeamMaker(this));
            _strategy.Add(TeamMakerStrategy.Manual, new ManualTeamMaker(this));
        }

        public async Task<TeamResult> MakeTeam(List<MemberName> members, int teamCount, TeamMakerStrategy strategy)
        {
            _teamResultList = null;

            if (_strategy.ContainsKey(strategy))
            {
                var teamMaker = _strategy[strategy];
                var result = await teamMaker.MakeTeamAsync(members, teamCount);
                return result;
            }

            return null;
        }

        public async Task<List<TeamResult>> GetAllTeamResult()
        {
            var cache = _teamResultList;
            if (cache != null)
                return cache;

            var files = await _fs.GetFilesAsync(path => path.GetPath(PathType.SuFcTeamsPath));

            var list = await files.Select(async file =>
                {
                    return await _fs.ReadJsonAsync<TeamResult>(path => path.GetPath(PathType.SuFcTeamsPath) + "/" + file);
                })
                .WhenAll();

            cache = list.ToList();
            _teamResultList = cache;
            return cache;
        }

        public async Task<TeamResult> FindTeamResult(string title)
        {
            var found = _teamResultList?.Find(x => x.Title == title);
            if (found != null)
                return found;

            var fileName = $"{title}.json";
            var saveFile = await _fs.ReadJsonAsync<TeamResult>(path => path.GetPath(PathType.SuFcTeamsPath) + "/" + fileName);
            return saveFile;
        }

        public async Task<bool> SaveTeamResult(TeamResult saveFile)
        {
            var fileName = $"{saveFile.Title}.json";

            if (fileName.HasInvalidFileNameChar())
            {
                return false;
            }

            await _fs.WriteJsonAsync(path => path.GetPath(PathType.SuFcTeamsPath) + "/" + fileName, saveFile);

            _teamResultList = null;

            return true;
        }
    }
}
