using Common.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace HelloJkwService.Reporra
{
    public interface IReporraRoom
    {
        string Id { get; }
        string RoomName { get; }
        ReporraGame Game { get; }
        ReporraRoomStatus Status { get; }
        IEnumerable<IReporraUser> GetPlayers();
        IEnumerable<IReporraUser> GetSpectators();
        IEnumerable<IReporraUser> GetAllUsers();
        TypedResult<IReporraUser> FindUserById(string id);
        TypedResult<IReporraUser> FindUserByCode(string code);

        Result EnterUserToPlayer(IReporraUser user);
        Result EnterUserToSpectator(IReporraUser user);
        Result LeaveUser(IReporraUser user);

        Result CreateGame();
        Result StartGame();
    }
}
