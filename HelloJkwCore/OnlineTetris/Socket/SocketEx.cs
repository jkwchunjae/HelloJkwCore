using Newtonsoft.Json;
using OnlineTetris.Packet;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineTetris.Socket
{
    public interface ISocketEx
    {
        Task<(int MessageLength, string Message)> ReceiveMessageAsync(CancellationToken ct = default);
        Task SendMessageAsync<T>(T message, CancellationToken ct = default) where T : PacketBase;
    }
    public class SocketEx : ISocketEx, IDisposable
    {
        private System.Net.Sockets.Socket _socket;
        private CircularArray _recvBuffer = new CircularArray(10000);
        private byte[] _messageLengthBuffer = new byte[sizeof(int)];
        private byte[] _sendBuffer = new byte[10000];

        private AsyncLock _sendLock = new AsyncLock();

        public bool Connected => _socket?.Connected ?? false;

        public SocketEx(System.Net.Sockets.Socket socket)
        {
            _socket = socket;
        }

        public void Dispose()
        {
            _socket?.Dispose();
        }

        public void Disconnect(bool reuseSocket)
        {
            _socket?.Disconnect(reuseSocket);
        }

        public void Close()
        {
            _socket?.Close();
        }

        public async Task<(int MessageLength, string Message)> ReceiveMessageAsync(CancellationToken ct = default)
        {
            var messageLength = await ReadMessageLengthAsync(ct);
            if (messageLength == 0)
            {
                return (0, string.Empty);
            }

            var message = await ReadMessageAsync(messageLength, ct);
            return (messageLength, message);
        }

        private async Task<int> ReadMessageLengthAsync(CancellationToken ct)
        {
            while (_recvBuffer.Length < sizeof(int))
            {
                await FillRecvBufferAsync(ct);
            }
            _recvBuffer.Read(_messageLengthBuffer);
            var length = BitConverter.ToInt32(_messageLengthBuffer);
            return length;
        }

        private async Task<string> ReadMessageAsync(int messageLength, CancellationToken ct)
        {
            while (_recvBuffer.Length < messageLength)
            {
                await FillRecvBufferAsync(ct);
            }
            var message = _recvBuffer.ReadString(messageLength);
            return message;
        }

        private async Task<int> FillRecvBufferAsync(CancellationToken ct)
        {
            var memory = _recvBuffer.GetWritableMemory();
            var count = await _socket.ReceiveAsync(memory, SocketFlags.None, ct);
            _recvBuffer.ShiftEndIndex(count);

            return count;
        }

        public async Task SendMessageAsync<T>(T message, CancellationToken ct = default) where T : PacketBase
        {
            var json = JsonConvert.SerializeObject(message);
            await SendTextAsync(json, ct);
        }

        private async Task SendTextAsync(string text, CancellationToken ct)
        {
            using (await _sendLock.LockAsync())
            {
                var byteLength = Encoding.UTF8.GetBytes(text, 0, text.Length, _sendBuffer, sizeof(int));
                _sendBuffer[0] = (byte)((byteLength >> 0) & 255);
                _sendBuffer[1] = (byte)((byteLength >> 8) & 255);
                _sendBuffer[2] = (byte)((byteLength >> 16) & 255);
                _sendBuffer[3] = (byte)((byteLength >> 24) & 255);

                await _socket.SendAsync(new Memory<byte>(_sendBuffer, 0, byteLength + sizeof(int)), SocketFlags.None, ct);
            }
        }
    }
}
