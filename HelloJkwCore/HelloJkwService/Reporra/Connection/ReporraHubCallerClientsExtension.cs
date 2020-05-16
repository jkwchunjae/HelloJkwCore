using Microsoft.AspNetCore.SignalR;

namespace HelloJkwService.Reporra
{
    public static class ReporraHubCallerClientsExtension
    {
        public static IClientProxy Group(this IHubCallerClients clients, Group group)
        {
            return clients.Group(group.Name);
        }
    }
}
