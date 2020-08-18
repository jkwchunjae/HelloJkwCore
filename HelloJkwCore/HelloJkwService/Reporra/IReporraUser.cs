using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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

        event EventHandler<ReporraGame> OnGameUpdated;
        void UpdateGame(ReporraGame game);

        Task<ReporraColor> WaitUserChoiceAsync(int size, string color);
    }
}
