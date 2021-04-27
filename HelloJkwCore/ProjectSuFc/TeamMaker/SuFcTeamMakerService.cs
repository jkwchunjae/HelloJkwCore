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

        public Task<TeamResult> MakeTeam(List<Member> members, int teamCount, TeamMakerStrategy strategy)
        {
            if (_strategy.ContainsKey(strategy))
            {
                var teamMaker = _strategy[strategy];
                var result = teamMaker.MakeTeam(members, teamCount);
                return Task.FromResult(result);
            }

            return Task.FromResult((TeamResult)null);
        }

        public async Task<List<TeamResultSaveFile>> GetAllTeamResult()
        {
            var files = await _fs.GetFilesAsync(path => path.GetPath(PathType.SuFcTeamsPath));

            var list = await files.Select(async file =>
                {
                    return await _fs.ReadJsonAsync<TeamResultSaveFile>(path => path.GetPath(PathType.SuFcTeamsPath) + "/" + file);
                })
                .WhenAll();

            return list.ToList();
        }

        public async Task<TeamResultSaveFile> FindTeamResult(string title)
        {
            var fileName = $"{title}.json";
            var saveFile = await _fs.ReadJsonAsync<TeamResultSaveFile>(path => path.GetPath(PathType.SuFcTeamsPath) + "/" + fileName);
            return saveFile;
        }

        public async Task<bool> SaveTeamResult(TeamResultSaveFile saveFile)
        {
            var fileName = $"{saveFile.Title}.json";

            if (fileName.HasInvalidFileNameChar())
            {
                return false;
            }

            await _fs.WriteJsonAsync(path => path.GetPath(PathType.SuFcTeamsPath) + "/" + fileName, saveFile);

            return true;
        }
    }
}
