using HelloJkwService.Reporra;
using HelloJkwService.User;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace HelloJkwClient.Pages.Reporra
{
    public partial class ReporraLobby : ComponentBase, IDisposable
    {
        [CascadingParameter]
        private AuthenticationState auth { get; set; }
        public bool IsAuthenticated { get; set; }

        private HubConnection _hubConnection;

        public string CreateRoomName;

        public IReporraUser _user;

        protected override async Task OnInitializedAsync()
        {
            auth = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            IsAuthenticated = auth.User.Identity.IsAuthenticated;

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(NavigationManager.ToAbsoluteUri("/reporraHub"))
                .Build();

            await _hubConnection.StartAsync();

            InitEventHandler();

            await InitReporraAsync();
        }

        public void Dispose()
        {
            Lobby.LeaveUser(_user);
            _hubConnection.DisposeAsync();
        }

        void InitEventHandler()
        {
            _hubConnection.On<string>(ClientApiName.UserEntered, (enteredUserId) =>
            {
                if (_user.Id == enteredUserId)
                    return;

                this.StateHasChanged();
            });

            _hubConnection.On<string>(ClientApiName.UserLeaved, (leavedUserId) =>
            {
                if (_user.Id == leavedUserId)
                    return;

                this.StateHasChanged();
            });
        }

        async Task InitReporraAsync()
        {
            _user = await MakeUser();

            var result = Lobby.EnterUser(_user);
            if (result.IsFail)
            {
                return;
            }

            HubHelper.SetUser(_user);

            await _hubConnection.EnterLobby(_user);
        }

        async Task<IReporraUser> MakeUser()
        {
            if (IsAuthenticated)
            {
                var userId = auth.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = await UserStore.FindByIdAsync(userId, CancellationToken.None);

                return new ReporraUser(user);
            }
            else
            {
                return new ReporraUser();
            }
        }

        void CreateRoom()
        {
            if (string.IsNullOrWhiteSpace(CreateRoomName))
            {
                return;
            }

            var result = Lobby.CreateRoom(new ReporraRoomOption
            {
                RoomName = CreateRoomName.Trim(),
            });

            if (result.IsFail)
            {
                return;
            }

            var room = result.Result;
            EnterRoom(room);
        }

        void EnterRoom(IReporraRoom room)
        {
            var result = room.EnterUserToPlayer(_user);
            if (result.IsFail)
            {
                return;
            }

            NavigationManager.NavigateTo($"/reporra/room/{room.Id}/{_user.Code}");
        }

        void SpectateRoom(IReporraRoom room)
        {
            var result = room.EnterUserToSpectator(_user);
            if (result.IsFail)
            {
                return;
            }

            NavigationManager.NavigateTo($"/reporra/room/{room.Id}/{_user.Code}");
        }
    }
}
