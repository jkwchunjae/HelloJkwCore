﻿namespace ProjectDiary.Pages;

public static class DiaryUrl
{
    public static string Home(DiaryName diaryName = null)
    {
        if (string.IsNullOrWhiteSpace(diaryName?.Name))
        {
            return $"/diary";
        }
        else
        {
            return $"/diary/{diaryName}";
        }
    }

    public static string Create()
    {
        return $"/diary/create";
    }

    public static string Write(DiaryInfo diaryInfo)
    {
        return $"/diary/write/{diaryInfo.DiaryName}";
    }

    public static string Write(DiaryInfo diaryInfo, DateTime date)
    {
        return $"/diary/write/{diaryInfo.DiaryName}/{date:yyyyMMdd}";
    }

    public static string Edit(DiaryInfo diaryInfo, DateTime date)
    {
        return $"/diary/edit/{diaryInfo.DiaryName}/{date:yyyyMMdd}";
    }

    public static string ShowAll(DiaryName diaryName)
    {
        return $"/diary/showall/{diaryName}";
    }

    public static string Search(DiaryName diaryName)
    {
        return $"/diary/search/{diaryName}";
    }

    public static string DiaryContent(DiaryName diaryName, DateTime date)
    {
        return $"/diary/{diaryName}/{date:yyyyMMdd}";
    }

    public static string PrevDate(DiaryView view)
    {
        if (view.DiaryNavigationData.HasPrev)
        {
            return DiaryContent(view.DiaryInfo.DiaryName, view.DiaryNavigationData.PrevDate.Value);
        }
        return string.Empty;
    }

    public static string NextDate(DiaryView view)
    {
        if (view.DiaryNavigationData.HasNext)
        {
            return DiaryContent(view.DiaryInfo.DiaryName, view.DiaryNavigationData.NextDate.Value);
        }
        return string.Empty;
    }

    public static string SetPassword()
    {
        return $"/diary/setpassword";
    }
}