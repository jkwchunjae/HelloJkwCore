using HelloJkwService.User;
using JkwExtensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace HelloJkwService.Reporra
{
    public class ReporraUser : IReporraUser
    {
        private ReporraUserType _userType = ReporraUserType.None;
        private readonly AppUser _user = null;
        private string _id = "";
        private string _userName = "";

        public string Id => _user?.Id ?? _id;
        public string Name => _user?.UserName ?? _userName;
        public bool IsAuthenticated => _user != null;
        public bool IsPlayer => _userType == ReporraUserType.Player;
        public bool IsSpectator => _userType == ReporraUserType.Spectator;
        public string Code { get; } = Guid.NewGuid().ToString("N").Substring(StaticRandom.Next(0, 20), StaticRandom.Next(6, 10));

        public ReporraUser()
        {
            var randomNumber = StaticRandom.Next(1111, 9999);
            _userName = $"User#{randomNumber}";
            _id = $"userid.{randomNumber}";
        }

        public ReporraUser(AppUser user)
        {
            _user = user;
        }

        public void ChangeUserType(ReporraUserType userType)
        {
            _userType = userType;
        }
    }
}
