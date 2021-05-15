using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineTetris.Socket
{
    public static class SocketExtension
    {
        public static async Task<(int ReceiveCount, string Text)> ReceiveTextAsync(this System.Net.Sockets.Socket socket, byte[] buffer, SocketFlags socketFlags = SocketFlags.None, CancellationToken cancellationToken = default)
        {
            var sb = new StringBuilder();
            var totalCount = 0;
            do
            {
                var count = await socket.ReceiveAsync(buffer, socketFlags, cancellationToken);
                totalCount += count;
                var text = Encoding.UTF8.GetString(buffer, 0, count);
                sb.Append(text);

            } while (socket.Available > 0);

            return (totalCount, sb.ToString());
        }

        public static async Task<T> ReceiveDataAsync<T>(this System.Net.Sockets.Socket socket, byte[] buffer, SocketFlags socketFlags = SocketFlags.None, CancellationToken cancellationToken = default)
        {
            // 이게 쓰일 일이 있을까?
            var (length, text) = await socket.ReceiveTextAsync(buffer, socketFlags, cancellationToken);
            var result = JsonConvert.DeserializeObject<T>(text);
            return result;
        }

        public static async Task SendTextAsync(this System.Net.Sockets.Socket socket, string text, SocketFlags socketFlags = SocketFlags.None, CancellationToken cancellationToken = default)
        {
            // 성능 포기하고 일단 돌아가게만 만들었음.
            var buffer = Encoding.UTF8.GetBytes(text);
            await socket.SendAsync(buffer, socketFlags, cancellationToken);
        }

        public static async Task SendDataAsync<T>(this System.Net.Sockets.Socket socket, T obj, SocketFlags socketFlags = SocketFlags.None, CancellationToken cancellationToken = default)
        {
            var json = JsonConvert.SerializeObject(obj);
            await socket.SendTextAsync(json, socketFlags, cancellationToken);
        }
    }
}
