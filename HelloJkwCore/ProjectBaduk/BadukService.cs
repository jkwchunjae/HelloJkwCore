using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBaduk
{
    public class BadukService : IBadukService
    {
        public Task DeleteBadukGameData(AppUser user, SummaryData summaryData)
        {
            throw new NotImplementedException();
        }

        public Task DeleteBadukGameData(AppUser user, string subject)
        {
            throw new NotImplementedException();
        }

        public Task<BadukGameData> GetBadukGameData(AppUser user, SummaryData summaryData)
        {
            throw new NotImplementedException();
        }

        public Task<BadukGameData> GetBadukGameData(AppUser user, string subject)
        {
            throw new NotImplementedException();
        }

        public Task<List<SummaryData>> GetBadukSummaryList(AppUser user)
        {
            throw new NotImplementedException();
        }

        public Task SaveBadukGameData(AppUser user, BadukGameData badukGameData)
        {
            throw new NotImplementedException();
        }
    }
}
