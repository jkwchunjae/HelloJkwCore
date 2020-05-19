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
        string Id { get; }
        string Name { get; }
        bool IsAuthenticated { get; }
        bool IsPlayer { get; }
        bool IsSpectator { get; }
        string Code { get; }
        void ChangeUserType(ReporraUserType userType);
    }
}
