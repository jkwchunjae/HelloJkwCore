﻿namespace HelloJkwCore.User;

public static class UserPathType
{
    public static readonly string UsersPath = nameof(UsersPath);
    public static readonly string UserDeletedPath = nameof(UserDeletedPath);
}
public static class UserPath
{
    public static string UserFilePathByFileName(this Paths path, string fileName)
    {
        return $"{path[UserPathType.UsersPath]}/{fileName}".ToLower();
    }

    public static string UserFilePathByUserId(this Paths path, UserId userId)
    {
        return $"{path[UserPathType.UsersPath]}/user.{userId}.json".ToLower();
    }

    public static string DeletedUserFilePathByUserId(this Paths path, UserId userId)
    {
        return $"{path[UserPathType.UserDeletedPath]}/user.{userId}.json".ToLower();
    }
}