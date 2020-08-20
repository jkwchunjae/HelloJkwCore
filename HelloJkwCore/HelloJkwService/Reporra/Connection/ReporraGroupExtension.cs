using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace HelloJkwService.Reporra
{
    public static class ReporraGroupExtension
    {
        public static async Task SendUserEntered(this Group group, IHubCallerClients clients, IReporraUser user)
        {
            await clients.Group(group).SendUserEntered(user);
        }

        public static async Task SendUserLeaved(this Group group, IHubCallerClients clients, IReporraUser user)
        {
            await clients.Group(group).SendUserLeaved(user);
        }

        public static async Task SendGameUpdated(this Group group, IHubCallerClients clients)
        {
            await clients.Group(group).SendGameUpdated();
        }
    }
}
