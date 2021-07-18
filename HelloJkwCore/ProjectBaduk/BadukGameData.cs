using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBaduk
{
    /// <summary> 바둑 한 판의 정보를 담고있는 데이터 </summary>
    public class BadukGameData
    {
        public string Subject { get; set; }
        public bool Favorite { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime LastModifyTime { get; set; }
        public string OwnerEmail { get; set; }
        public int Size { get; set; }
        public StoneChangeMode ChangeMode { get; set; }
        public StoneColor CurrentColor { get; set; }
        public int CurrentIndex { get; set; }
        public int LastIndex { get; set; }
        public string Memo { get; set; }
        public List<StoneLogData> StoneLog { get; set; }
    }

    public class SummaryData
    {
        public string Subject { get; set; }
        public bool Favorite { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public class StoneLogData
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public StoneAction Action { get; set; }
        public StoneColor Color { get; set; }
    }

    public enum StoneColor
    {
        None, Black, White
    }

    public enum StoneAction
    {
        Set, Remove
    }

    public enum StoneChangeMode
    {
        Auto, Menual
    }

}
