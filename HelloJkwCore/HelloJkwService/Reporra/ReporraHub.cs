using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloJkwService.Reporra
{
    public class ReporraHub : Hub
    {
        private readonly IReporraLobbyService _lobby;

        public ReporraHub(IReporraLobbyService lobbyService)
            : base()
        {
            _lobby = lobbyService;
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.All.UpdateLobbyAsync();
            await base.OnDisconnectedAsync(exception);
        }

        public async Task UpdateLobby()
        {
            await Clients.All.UpdateLobbyAsync();
        }
    }

    public static class ReporraClientProxyExtension
    {
        public static async Task UpdateLobbyAsync(this IClientProxy clientProxy)
        {
            await clientProxy.SendAsync("UpdateLobby");
        }
    }

    public static class HubConnectionExtension
    {
        public static async Task SendUpdateLobby(this HubConnection hubConnection)
        {
            await hubConnection.SendAsync("UpdateLobby");
        }
    }
}
