using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

namespace HelloJkwService.Reporra
{
    public static class HubConnectionExtension
    {
        private static async Task EnterGroup(this HubConnection hubConnection, string groupName, IReporraUser user)
        {
            await hubConnection.SendAsync(ServerApiName.EnterGroup, groupName, user.Id);
        }

        public static async Task EnterLobby(this HubConnection hubConnection, IReporraUser user)
        {
            await hubConnection.EnterGroup(Group.Lobby.Name, user);
        }

        public static async Task EnterRoom(this HubConnection hubConnection, IReporraRoom room, IReporraUser user)
        {
            var groupName = room.Id;
            await hubConnection.EnterGroup(groupName, user);
        }

        public static async Task CreateGame(this HubConnection hubConnection, IReporraRoom room)
        {
            await hubConnection.SendAsync(ServerApiName.CreateGame, room.Id);
        }
    }
}
