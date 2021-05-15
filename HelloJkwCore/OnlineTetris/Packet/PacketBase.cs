using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineTetris.Packet
{

    public class PacketBase
    {
        public PacketType Type { get; private set; }

        public PacketBase(PacketType type)
        {
            Type = type;
        }
    }
}
