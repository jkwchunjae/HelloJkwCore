using System;

namespace ProjectDiary
{
    public class DiaryNavigationData
    {
        public DateTime Today { get; set; }
        public DateTime? PrevDate { get; set; }
        public DateTime? NextDate { get; set; }
        public bool HasPrev => PrevDate.HasValue;
        public bool HasNext => NextDate.HasValue;
    }
}
