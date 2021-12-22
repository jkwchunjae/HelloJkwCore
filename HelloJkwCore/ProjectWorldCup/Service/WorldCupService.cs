using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectWorldCup
{
    public class WorldCupService : IWorldCupService
    {
        private IFifa _fifa;

        public WorldCupService(IFifa fifa)
        {
            _fifa = fifa;
        }

        public Task<List<QualifiedTeam>> Get2022QualifiedTeamsAsync()
        {
            return _fifa.GetQualifiedTeamsAsync();
        }
    }
}
