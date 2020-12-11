using System;
using System.Linq;
using System.Threading.Tasks;

namespace HelloJkwCore.User
{
    public class AppUser
    {
        public string Id { get; set; }
        public DateTime CreateTime { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        public AppUser() { }

        public AppUser(string loginProvider, string providerKey)
        {
            Id = UserId(loginProvider, providerKey);
        }

        public static string UserId(string loginProvider, string providerKey)
        {
            return $"{loginProvider}.{providerKey}";
        }

    }
}
