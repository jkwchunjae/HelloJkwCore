using System;
using System.Collections.Generic;
using System.Text;

namespace HelloJkwService.Reporra
{
    public class ReporraUser : IReporraUser
    {
        private ReporraUserType _userType;
        private string _userName;

        public bool IsPlayer() => _userType == ReporraUserType.Player;
        public bool IsSpectator() => _userType == ReporraUserType.Spectator;

        public ReporraUser()
        {
            _userName = "User#" + (new Random().Next(1111, 9999)).ToString();
        }

        public ReporraUser(string userName)
        {
            _userName = userName;
        }

        public string GetUserName()
        {
            return _userName;
        }

        public void ChangeUserType(ReporraUserType userType)
        {
            _userType = userType;
        }

    }
}
