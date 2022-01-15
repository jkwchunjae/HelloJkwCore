﻿namespace ProjectSuFc;

public partial class SuFcService : ISuFcService
{
    private List<Member> _memberList = null;
    public async Task<List<Member>> GetAllMember()
    {
        var cache = _memberList;
        if (cache != null)
            return cache;

        var files = await _fs.GetFilesAsync(path => path[SuFcPathType.SuFcMembersPath]);

        var list = await files.Select(async file =>
            {
                return await _fs.ReadJsonAsync<Member>(path => path[SuFcPathType.SuFcMembersPath] + "/" + file);
            })
            .WhenAll();

        cache = list.ToList();
        _memberList = cache;

        return cache;
    }

    public async Task<Member> FindMember(MemberName memberName)
    {
        var fileName = $"{memberName}.json";

        var member = await _fs.ReadJsonAsync<Member>(path => path[SuFcPathType.SuFcMembersPath] + "/" + fileName);

        return member;
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

        var result = await _fs.WriteJsonAsync(path => path[SuFcPathType.SuFcMembersPath] + "/" + fileName, member);

        _memberList = null;

        return result;
    }
}