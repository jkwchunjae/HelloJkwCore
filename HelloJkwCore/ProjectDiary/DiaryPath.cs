namespace ProjectDiary;

public static class DiaryPathType
{
    public static readonly string DiaryNameListFile = nameof(DiaryNameListFile);
    public static readonly string DiaryContentsRootPath = nameof(DiaryContentsRootPath);
    public static readonly string DiaryListPath = nameof(DiaryListPath);
    public static readonly string DiaryTriePath = nameof(DiaryTriePath);
}

public static class DiaryPath
{
    private static string DiaryInfoList(this Paths paths)
    {
        return paths[DiaryPathType.DiaryListPath];
    }

    public static string DiaryContents(this Paths paths)
    {
        return paths[DiaryPathType.DiaryContentsRootPath];
    }

    public static string DiaryNameListFile(this Paths paths)
    {
        return paths[DiaryPathType.DiaryNameListFile];
    }

    public static string UserDiaryInfo(this Paths paths, AppUser user)
    {
        return $"{paths.DiaryInfoList()}/userdiary.{user.Id}.json";
    }

    public static string UserDiaryInfo(this Paths paths, UserId userId)
    {
        return $"{paths.DiaryInfoList()}/userdiary.{userId}.json";
    }

    public static string DiaryInfo(this Paths paths, DiaryName diaryName)
    {
        return $"{paths.DiaryInfoList()}/diary.{diaryName}.json";
    }

    public static string Diary(this Paths paths, DiaryName diaryName)
    {
        return $"{paths.DiaryContents()}/{diaryName}";
    }

    public static string Content(this Paths paths, DiaryName diaryName, string fileName)
    {
        return $"{paths.Diary(diaryName)}/{fileName}";
    }

    public static string DiaryTrie(this Paths paths, DiaryName diaryName)
    {
        var dirPath = paths[DiaryPathType.DiaryTriePath];
        return $"{dirPath}/trie.{diaryName}.json";
    }
}