using System.Collections.Generic;

namespace ProjectDiary
{
    public class DiaryInfo
    {
        public string Id { get; set; }
        public string Owner { get; set; }
        public string DiaryName { get; set; }
        public bool IsSecret { get; set; }
        public List<string> Writers { get; set; }
        public List<string> Viewers { get; set; }

        public DiaryInfo()
        {
            Writers = new List<string>();
            Viewers = new List<string>();
        }

        public DiaryInfo(string id, string owner, string diaryName, bool isSecret)
            : this()
        {
            Id = id;
            Owner = owner;
            DiaryName = diaryName;
            IsSecret = isSecret;
        }
    }
}
