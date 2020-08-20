using HelloJkwService.Reporra;
using JkwExtensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HelloJkwClient.Pages.Reporra
{
    public partial class ReporraRoom: ComponentBase, IDisposable
    {
        [Parameter]
        public string RoomId { get; set; }
        [Parameter]
        public string UserCode { get; set; }

        [Inject]
        AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }
        [Inject]
        IReporraLobbyService Lobby { get; set; }
        [Inject]
        IReporraHubHelper HubHelper { get; set; }

        [CascadingParameter]
        private AuthenticationState auth { get; set; }
        public bool IsAuthenticated { get; set; }

        private HubConnection _hubConnection;

        IReporraRoom _room;
        IReporraUser _user;
        ReporraGame _game;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            var result = Lobby.FindRoomById(RoomId);

            if (result.IsFail)
            {
                return;
            }
            _room = result.Result;

            auth = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            IsAuthenticated = auth.User.Identity.IsAuthenticated;

            _user = GetUser();

            if (_user == null)
            {
                return;
            }

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(NavigationManager.ToAbsoluteUri("/reporraHub"))
                .Build();

            await _hubConnection.StartAsync();

            InitEventHandler();

            await InitReporraAsync();
        }

        public void Dispose()
        {
            _room?.LeaveUser(_user);
            _hubConnection.DisposeAsync();
        }

        void InitEventHandler()
        {
            _hubConnection.On<string>(ClientApiName.UserEntered, (enteredUserId) =>
            {
                if (_user.Id == enteredUserId)
                    return;

                StateHasChanged();
            });

            _hubConnection.On<string>(ClientApiName.UserLeaved, (leavedUserId) =>
            {
                if (_user.Id == leavedUserId)
                    return;

                StateHasChanged();
            });

            _hubConnection.On(ClientApiName.GameUpdated, () =>
            {
                _game = _room.Game;
                StateHasChanged();
            });
        }

        private IReporraUser GetUser()
        {
            if (IsAuthenticated)
            {
                var userId = auth.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var result = _room.FindUserById(userId);
                if (result.IsSuccess)
                {
                    return result.Result;
                }
            }
            else
            {
                var result = _room.FindUserByCode(UserCode);
                if (result.IsSuccess)
                {
                    return result.Result;
                }
            }
            return null;
        }

        async Task InitReporraAsync()
        {
            HubHelper.SetUser(_user);

            await _hubConnection.EnterRoom(_room, _user);
        }

        async Task InitGameAsync()
        {
            await _hubConnection.CreateGame(_room);
        }
    }
}
