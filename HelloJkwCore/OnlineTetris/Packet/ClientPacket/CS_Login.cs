using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineTetris.Packet
{
    public class CS_Login : PacketBase
    {
        public string UserName { get; set; }

        public CS_Login()
            : base(PacketType.CS_Login)
        {
        }
    }

    public class CS_Start : PacketBase
    {
        public CS_Start()
            : base(PacketType.CS_Start)
        {
        }
    }
}
