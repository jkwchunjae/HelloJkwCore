using Common;
using Microsoft.AspNetCore.Identity;

namespace ProjectWorldCup;

public partial class BettingService : IBettingService
{
    private readonly IFileSystem _fs;

    public BettingService(
        IFileSystemService fsService,
        WorldCupOption option,
        ICacheClearInvoker cacheClearInvoker,
        IUserStore<AppUser> userStore)
    {
        _fs = fsService.GetFileSystem(option.FileSystemSelect, option.Path);
        _fs2018 = fsService.GetFileSystem(option.FileSystemSelect2018, option.Path);
        _userStore = userStore;

        cacheClearInvoker.ClearCacheInvoked += (_, _) =>
        {
            ClearUserCache();
        };
    }

    public async Task<BettingUser> JoinBettingAsync(BettingUser buser, BettingType bettingType)
    {
        var user = await GetBettingUserAsync(buser.AppUser);

        if (user.JoinStatus != UserJoinStatus.Joined)
        {
            throw new Exception("참가신청을 먼저 해야 합니다.");
        }

        user.JoinedBetting ??= new();
        user.JoinedBetting.Add(bettingType);

        var bettingName = bettingType == BettingType.GroupStage ? "16강 진출팀 맞추기"
            : bettingType == BettingType.Round16 ? "8강 진출팀 맞추기"
            : "1,2,3,4 등 맞추기";
        user.BettingHistories.Add(new BettingHistory
        {
            Type = HistoryType.Betting,
            Value = -10000,
            Comment = $"'{bettingName}'내기에 참가했습니다.",
        });
        await SaveUserAsync(user);

        return user;
    }
}
