using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public partial class PathOf
    {
        private string DiaryInfoList()
        {
            return _pathDic[PathType.DiaryListPath];
        }

        public string DiaryContents()
        {
            return _pathDic[PathType.DiaryContentsRootPath];
        }

        public string DiaryNameListFile()
        {
            return _pathDic[PathType.DiaryNameListFile];
        }

        public string UserDiaryInfo(AppUser user)
        {
            return $"{DiaryInfoList()}/userdiary.{user.Id}.json";
        }

        public string DiaryInfo(string diaryName)
        {
            return $"{DiaryInfoList()}/diary.{diaryName}.json";
        }

        public string Diary(string diaryName)
        {
            return $"{DiaryContents()}/{diaryName}";
        }

        public string Content(string diaryName, string fileName)
        {
            return $"{Diary(diaryName)}/{fileName}";
        }
    }
}
