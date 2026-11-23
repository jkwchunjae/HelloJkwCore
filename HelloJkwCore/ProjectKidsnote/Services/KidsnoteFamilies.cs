using Common;
using Microsoft.Extensions.DependencyInjection;
using ProjectKidsnote.Models.Parents;

namespace ProjectKidsnote.Services;

internal sealed class KidsnoteFamilies(
    [FromKeyedServices(nameof(KidsnoteService))] IFileSystem files)
{
    private readonly SemaphoreSlim gate = new(1);
    private static string Folder(Paths paths) => paths["ParentPath"];

    public async Task<List<KidsParent>> ReadAsync()
    {
        await gate.WaitAsync();
        try { return await ReadFilesAsync(); }
        finally { gate.Release(); }
    }

    private async Task<List<KidsParent>> ReadFilesAsync()
    {
        List<KidsParent> parents = [];
        if (!await files.DirExistsAsync(Folder)) return parents;
        foreach (var name in await files.GetFilesAsync(Folder, ".json"))
        {
            if (name.StartsWith("kidsnote.", StringComparison.Ordinal) && name.EndsWith(".json", StringComparison.Ordinal))
                parents.Add(await files.ReadJsonAsync<KidsParent>(p => $"{Folder(p)}/{name}"));
        }
        return parents;
    }

    public async Task SaveAsync(AppUser actor, KidsParent draft)
    {
        await gate.WaitAsync();
        try
        {
            var parents = await ReadFilesAsync();
            var saved = parents.SingleOrDefault(p => p.User == draft.User);
            if (!(saved?.CanManage(actor.Id) ?? (actor.HasRole(UserRole.Kidsnote) && actor.Id == draft.User)))
                throw new InvalidOperationException("가족 설정을 수정할 권한이 없습니다.");
            if ((saved?.Revision ?? 0) != draft.Revision)
                throw new InvalidOperationException("가족 설정이 변경되었습니다. 화면을 새로고침해 주세요.");
            if (string.IsNullOrEmpty(draft.Auth?.EncryptedAuthInfo))
                throw new InvalidOperationException("먼저 키즈노트 계정을 연결해 주세요.");
            var names = parents.Where(p => p.User != draft.User).SelectMany(p => p.Kids)
                .Select(k => k.NicknameForUrl).ToHashSet(StringComparer.OrdinalIgnoreCase);
            foreach (var kid in draft.Kids)
            {
                var name = kid.NicknameForUrl;
                if (string.IsNullOrEmpty(name) || !name.All(c => c is >= 'a' and <= 'z' or >= '0' and <= '9'))
                    throw new InvalidOperationException("URL 닉네임은 영문 소문자와 숫자만 사용할 수 있습니다.");
                if (!names.Add(name))
                    throw new InvalidOperationException($"이미 사용 중인 URL 닉네임입니다. ({name})");
            }

            await files.CreateDirectoryAsync(Folder);
            draft.Revision++;
            try
            {
                if (!await files.WriteJsonAsync(p => $"{Folder(p)}/kidsnote.{draft.User}.json", draft))
                    throw new IOException("가족 설정을 저장하지 못했습니다.");
            }
            catch { draft.Revision--; throw; }
        }
        finally { gate.Release(); }
    }
}
