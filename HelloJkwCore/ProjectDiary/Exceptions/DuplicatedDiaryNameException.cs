using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDiary
{
    class DuplicatedDiaryNameException : Exception
    {
        public readonly string DiaryName;

        public DuplicatedDiaryNameException(string diaryName)
        {
            DiaryName = diaryName;
        }

    }
}
