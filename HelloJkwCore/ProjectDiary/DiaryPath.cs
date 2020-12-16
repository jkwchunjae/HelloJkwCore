using Common;
using Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDiary
{
    public static class DiaryPath
    {
        public static string DiaryList()
        {
            return PathType.DiaryListFile.GetPath();
        }

        public static string Root()
        {
            return PathType.DiaryRootPath.GetPath();
        }

        public static string Diary(string diaryName)
        {
            return $"{Root()}/{diaryName}";
        }

        public static string Content(string diaryName, string fileName)
        {
            return $"{Diary(diaryName)}/{fileName}";
        }
    }
}
