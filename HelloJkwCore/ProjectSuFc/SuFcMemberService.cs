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
        private async Task SaveMembers(List<Member> members)
        {
            var sorted = members
                .OrderBy(x => x.RegisterDate)
                .ToList();
            var result = await _fs.WriteJsonAsync(path => path.GetPath(PathType.SuFcMemberListFile), sorted);
        }
        public async Task<Member> AddMember(Member member)
        {
            var list = await GetMembers();

            if (list.Any(x => x.Name == member.Name))
            {
                return null;
            }

            member.No = list.MaxOrNull(x => x.No) + 1 ?? 1;

            list.Add(member);

            await SaveMembers(list);
            return member;
        }

        public async Task<bool> DeleteMember(Member member)
        {
            var list = await GetMembers();

            list = list.Where(x => x.Name != member.Name).ToList();

            await SaveMembers(list);
            return true;
        }

        public async Task<Member> FindMember(string name)
        {
            var list = await GetMembers();
            return list.FirstOrDefault(x => x.Name == name);
        }

        public async Task<List<Member>> GetMembers()
        {
            var list = await _fs.ReadJsonAsync<List<Member>>(path => path.GetPath(PathType.SuFcMemberListFile));

            return list ?? new List<Member>();
        }

        public async Task<Member> UpdateMember(Member member)
        {
            var list = await GetMembers();

            list = list.Where(x => x.Name != member.Name)
                .Concat(new[] { member })
                .ToList();

            await SaveMembers(list);
            return member;
        }
    }
}
