namespace OnlineTetris.Packet
{
    public enum PacketType
    {
        CS_Login,
        CS_Start,
        CS_GetNextPiece,
        CS_Board,

        SC_LoginAllow,
        SC_MemberUpdated,
        SC_Start,
        SC_NextPiece,
        SC_BoardUpdated,
    }
}
