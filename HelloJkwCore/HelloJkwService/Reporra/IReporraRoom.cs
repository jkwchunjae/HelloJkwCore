using Common.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace HelloJkwService.Reporra
{
    public interface IReporraRoom
    {
        string GetRoomName();
        ReporraRoomStatus GetStatus();
        IEnumerable<IReporraUser> GetPlayers();
        IEnumerable<IReporraUser> GetSpectators();
        IEnumerable<IReporraUser> GetAllUsers();

        Result EnterUserToPlayer(IReporraUser user);
        Result EnterUserToSpectator(IReporraUser user);
        Result LeaveUser(IReporraUser user);
        Result StartGame();
    }
}
