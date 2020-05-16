using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

namespace HelloJkwService.Reporra
{
    public static class HubConnectionExtension
    {
        private static async Task EnterGroup(this HubConnection hubConnection, Group group, IReporraUser user)
        {
            await hubConnection.SendAsync(ServerApiName.EnterGroup, group.Name, user.Id);
        }

        public static async Task EnterLobby(this HubConnection hubConnection, IReporraUser user)
        {
            await hubConnection.EnterGroup(Group.Lobby, user);
        }
    }
}
