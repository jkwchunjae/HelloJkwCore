using System;
using System.Collections.Generic;
using System.Text;

namespace HelloJkwService.Reporra
{
    public class ReporraUser : IReporraUser
    {
        private ReporraUserType _userType;

        public bool IsPlayer() => _userType == ReporraUserType.Player;
        public bool IsSpectator() => _userType == ReporraUserType.Spectator;

        public string GetUserName()
        {
            throw new NotImplementedException();
        }

        public void ChangeUserType(ReporraUserType userType)
        {
            _userType = userType;
        }

    }
}
