using Newtonsoft.Json.Linq;
using OnlineTetris.Packet;
using OnlineTetris.Socket;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace OnlineTetris
{
    public class TetrisClient
    {
        private SocketEx connection;

        public event EventHandler<SC_LoginAllow> OnLoginAllow;
        public event EventHandler<SC_MemberUpdated> OnMemberUpdated;
        public event EventHandler<SC_Start> OnStart;
        public event EventHandler<SC_NextPiece> OnNextPiece;

        public async Task Connect(IPAddress ip, string name)
        {
            if (connection?.Connected ?? false)
            {
                connection.Disconnect(false);
                connection.Close();
                connection = null;
            }
            //IPAddress ipAddress = IPAddress.Parse("221.143.21.37");
            IPEndPoint remoteEP = new IPEndPoint(ip, 52217);

            // Create a TCP/IP socket.  
            try
            {
                var server = new System.Net.Sockets.Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                await server.ConnectAsync(remoteEP);

                //richTextBox1.Text += $"Connected: {server.Connected} \n";

                connection = new SocketEx(server);

                await connection.SendMessageAsync(new CS_Login
                {
                    UserName = name,
                });

                await HandleReceiveAsync();
            }
            catch (Exception ex)
            {
                connection?.Close();
                connection = null;
            }
        }

        public void Close()
        {
            connection?.Disconnect(false);
            connection.Close();
            connection = null;
        }

        private async Task HandleReceiveAsync()
        {
            while (true)
            {
                var (receiveCount, receiveText) = await connection.ReceiveMessageAsync();
                if (receiveCount == 0)
                {
                    if (connection.Connected)
                    {
                    }
                    else
                    {
                    }
                    break;
                }

                var obj = JObject.Parse(receiveText);
                if (!obj.ContainsKey("Type"))
                {
                    continue;
                }

                HandlePacket(obj);
            }
        }

        private void HandlePacket(JObject packetObj)
        {
            var packetType = Enum.Parse<PacketType>(packetObj.Value<string>("Type"));

            if (packetType == PacketType.SC_LoginAllow)
            {
                var packet = packetObj.ToObject<SC_LoginAllow>();
                OnLoginAllow?.Invoke(this, packet);
            }
            else if (packetType == PacketType.SC_MemberUpdated)
            {
                var packet = packetObj.ToObject<SC_MemberUpdated>();
                OnMemberUpdated?.Invoke(this, packet);
            }
            else if (packetType == PacketType.SC_Start)
            {
                var packet = packetObj.ToObject<SC_Start>();
                OnStart?.Invoke(this, packet);
            }
            else if (packetType == PacketType.SC_NextPiece)
            {
                var packet = packetObj.ToObject<SC_NextPiece>();
                OnNextPiece?.Invoke(this, packet);
            }
        }
    }
}
