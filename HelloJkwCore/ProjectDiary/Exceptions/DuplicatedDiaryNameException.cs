namespace ProjectDiary;

class DuplicatedDiaryNameException : Exception
{
    public readonly string DiaryName;

    public DuplicatedDiaryNameException(string diaryName)
    {
        DiaryName = diaryName;
    }

}