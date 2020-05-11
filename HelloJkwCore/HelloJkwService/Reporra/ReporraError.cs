using System;
using System.Collections.Generic;
using System.Text;

namespace HelloJkwService.Reporra
{
    public static class ReporraError
    {
        // Begin Common
        public readonly static string NotExist = "NotExist";
        // End Common

        // Begin Service
        public readonly static string DuplicatedName = "DuplicatedName";
        // End Service

        // Begin Room
        public readonly static string RoomIsFull = "RoomIsFull";
        public readonly static string AlreadyStart = "AlreadyStart";
        public readonly static string NotEnoughPlayer = "NotEnoughPlayer";
        // End Room
    }
}
