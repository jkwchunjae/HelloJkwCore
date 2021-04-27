using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JkwExtensions;

namespace ProjectSuFc
{
    public partial class SuFcService : ISuFcService
    {
        public async Task<List<Member>> GetAllMember()
        {
            var files = await _fs.GetFilesAsync(path => path.GetPath(PathType.SuFcMembersPath));

            var list = await files.Select(async file =>
                {
                    return await _fs.ReadJsonAsync<Member>(path => path.GetPath(PathType.SuFcMembersPath) + "/" + file);
                })
                .WhenAll();

            return list.ToList();
        }

        public async Task<bool> SaveMember(Member member)
        {
            var fileName = $"{member.Name}.json";

            if (fileName.HasInvalidFileNameChar())
            {
                return false;
            }

            var list = await GetAllMember();
            if (list.Empty(x => x.Name == member.Name))
            {
                member.No = list.MaxOrNull(x => x.No) + 1 ?? 1;
            }

            var result = await _fs.WriteJsonAsync(path => path.GetPath(PathType.SuFcMembersPath) + "/" + fileName, member);

            return result;
        }
    }
}
