using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloJkwService.Reporra
{
    public class ReporraHub : Hub
    {
        private readonly IReporraLobbyService _lobby;
        private readonly IReporraHubHelper _hubHelper;

        public ReporraHub(IReporraLobbyService lobbyService, IReporraHubHelper hubHelper)
            : base()
        {
            _lobby = lobbyService;
            _hubHelper = hubHelper;
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connectionId = Context.ConnectionId;

            if (_hubHelper.LeaveGroup(connectionId, out var group, out var user))
            {
                if (group != null)
                {
                    await Groups.RemoveFromGroupAsync(connectionId, group.Name);
                    await group.SendUserLeaved(Clients, user);
                }
                if (user != null)
                {
                    _hubHelper.RemoveUser(user.Id);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task EnterGroup(string groupName, string userId)
        {
            var group = _hubHelper.GetGroup(groupName);
            var user = _hubHelper.GetUser(userId);
            var connectionId = Context.ConnectionId;

            if (_hubHelper.IsInAnotherGroup(connectionId, out var prevGroup, out _))
            {
                await Groups.RemoveFromGroupAsync(connectionId, prevGroup.Name);
                await prevGroup.SendUserLeaved(Clients, user);
            }

            _hubHelper.EnterGroup(connectionId, group, user);
            await Groups.AddToGroupAsync(Context.ConnectionId, group.Name);

            await group.SendUserEntered(Clients, user);
        }

        public async Task CreateGame(string roomId, GameCreateOption createOption)
        {
            var result = _lobby.FindRoomById(roomId);
            if (result.IsFail)
                return;

            var room = result.Result;
            room.CreateGame(createOption);

            var group = _hubHelper.GetGroup(roomId);
            await group.SendGameUpdated(Clients);
        }
    }
}
