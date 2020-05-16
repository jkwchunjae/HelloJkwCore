using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

namespace HelloJkwService.Reporra
{
    public static class ReporraClientProxyExtension
    {
        public static async Task SendUserEntered(this IClientProxy clients, IReporraUser user)
        {
            await clients.SendAsync("UserEntered", user.Id);
        }

        public static async Task SendUserLeaved(this IClientProxy clients, IReporraUser user)
        {
            await clients.SendAsync("UserLeaved", user.Id);
        }
    }
}
