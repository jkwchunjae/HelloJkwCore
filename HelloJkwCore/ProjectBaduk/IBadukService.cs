using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBaduk
{
    public interface IBadukService
    {
        Task<List<BadukGameData>> GetBadukSummaryList(AppUser user);
        Task<BadukGameData> GetBadukGameData(AppUser user, string subject);
        Task SaveBadukGameData(AppUser user, BadukGameData badukGameData);
        Task DeleteBadukGameData(AppUser user, string subject);
    }
}
