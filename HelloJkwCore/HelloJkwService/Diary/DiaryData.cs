using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HelloJkwService.Diary
{
    public class DiaryData
    {
        public DateTime Date { get; set; }
        [JsonProperty("RegDate")]
        public DateTime CreateDate { get; set; }
        public DateTime LastModifyDate { get; set; }
        public bool IsSecure { get; set; }
        public int Index { get; set; }
        public string Text { get; set; }

        [JsonIgnore]
        private DiaryService _diaryService;
        [JsonIgnore]
        private DiaryInfo _diaryInfo;

        public void SetDiaryInfo(DiaryService diaryService, DiaryInfo diaryInfo)
        {
            _diaryService = diaryService;
            _diaryInfo = diaryInfo;
        }

        public bool HasPrev(out DateTime prevDate)
        {
            var diaryList = _diaryService.GetDiaryDataListFromCache(_diaryInfo.DiaryName);
            var lastDiary = diaryList.LastOrDefault(x => x.Date < Date);

            if (lastDiary != null)
            {
                prevDate = lastDiary.Date;
                return true;
            }
            else
            {
                prevDate = DateTime.MinValue;
                return false;
            }
        }

        public bool HasNext(out DateTime nextDate)
        {
            var diaryList = _diaryService.GetDiaryDataListFromCache(_diaryInfo.DiaryName);
            var firstDiary = diaryList.LastOrDefault(x => x.Date > Date);

            if (firstDiary != null)
            {
                nextDate = firstDiary.Date;
                return true;
            }
            else
            {
                nextDate = DateTime.MinValue;
                return false;
            }
        }
    }
}
