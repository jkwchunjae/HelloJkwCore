using System;
using System.Collections.Generic;
using System.Text;

namespace HelloJkwService.Diary
{
    public class DiaryInfo
    {
        public string Id { get; set; }
        public string Owner { get; set; }
        public string DiaryName { get; set; }
        public bool IsSecure { get; set; }
        public List<string> Writers { get; set; }
        public List<string> Viewers { get; set; }
    }
}
