using Common.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace HelloJkwService.Reporra
{
    public interface IReporraLobbyService
    {
        IEnumerable<IReporraRoom> GetRoomList();
        TypedResult<IReporraRoom> CreateRoom(ReporraRoomOption option);
        Result DeleteRoom(string roomName);

        IEnumerable<IReporraUser> GetUserList();
        Result EnterUser(IReporraUser user);
        Result LeaveUser(IReporraUser user);
    }
}
