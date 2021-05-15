using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineTetris.Packet
{
    public class ServerPacketBase : PacketBase
    {
        public DateTime Time { get; } = DateTime.Now;

        public ServerPacketBase(PacketType type)
            : base(type)
        {
        }
    }
    public class SC_LoginAllow : ServerPacketBase
    {
        public bool Allow { get; set; }
        public string DeniedReason { get; set; }

        public SC_LoginAllow()
            : base(PacketType.SC_LoginAllow)
        {
        }
    }

    public class SC_MemberUpdated : ServerPacketBase
    {
        public string Id { get; set; }
        public List<string> UserList { get; set; }

        public SC_MemberUpdated()
            : base (PacketType.SC_MemberUpdated)
        {
        }
    }

    public class SC_Start : ServerPacketBase
    {
        public SC_Start()
            : base(PacketType.SC_Start)
        {
        }
    }

    public class SC_NextPiece : ServerPacketBase
    {
        public PieceType PieceType { get; set; }
        public SC_NextPiece()
            : base(PacketType.SC_NextPiece)
        {
        }
    }
}
