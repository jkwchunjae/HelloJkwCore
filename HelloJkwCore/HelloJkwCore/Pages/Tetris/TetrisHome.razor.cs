using Common;
using HelloJkwCore.Shared;
using Microsoft.AspNetCore.Components;
using OnlineTetris;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace HelloJkwCore.Pages.Tetris
{
    public partial class TetrisHome : JkwPageBase
    {
        [Inject]
        ITetrisService TetrisService { get; set; }

        private TetrisClient Client { get; set; }

        private string Log { get; set; }

        async Task ConnectAsync()
        {
            Client = TetrisService.GetTetrisClient();

            Client.OnLoginAllow += Client_OnLoginAllow;
            Client.OnMemberUpdated += Client_OnMemberUpdated;

            await Client.Connect(IPAddress.Loopback, "경원");
        }

        protected override void OnPageDispose()
        {
            TetrisService.CloseTetrisClient(Client);
        }

        private void Client_OnMemberUpdated(object sender, OnlineTetris.Packet.SC_MemberUpdated e)
        {
            Log = "[MemberUpdated] " + Json.Serialize(e.UserList) + Environment.NewLine + Log;
            StateHasChanged();
        }

        private void Client_OnLoginAllow(object sender, OnlineTetris.Packet.SC_LoginAllow e)
        {
            Log = "[LoginAllow] " + Environment.NewLine + Log;
            StateHasChanged();
        }
    }
}
