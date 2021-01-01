using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDiary
{
    public class UserDiaryInfo
    {
        public string UserId { get; set; }
        public List<string> MyDiaryList { get; set; } = new();
        public List<string> WriterList { get; set; } = new();
        public List<string> ViewList { get; set; } = new();

        public bool IsMine(string diaryName)
        {
            return MyDiaryList.Contains(diaryName);
        }

        public bool IsWritable(string diaryName)
        {
            return IsMine(diaryName) || WriterList.Contains(diaryName);
        }

        public bool IsViewable(string diaryName)
        {
            return IsWritable(diaryName) || ViewList.Contains(diaryName);
        }

        public void AddMyDiary(string diaryName)
        {
            MyDiaryList.Add(diaryName);
        }
    }
}
