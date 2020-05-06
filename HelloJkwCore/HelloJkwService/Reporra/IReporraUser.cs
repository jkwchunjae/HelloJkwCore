using System;
using System.Collections.Generic;
using System.Text;

namespace HelloJkwService.Reporra
{
    public enum ReporraUserType
    {
        None,
        Player,
        Spectator,
    }

    public interface IReporraUser
    {
        string GetUserName();
        void ChangeUserType(ReporraUserType userType);
        bool IsPlayer();
        bool IsSpectator();
    }
}
