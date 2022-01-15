using Common;

namespace HelloJkwCore.User;

public static class UserPathType
{
    public static readonly string UsersPath = nameof(UsersPath);
}
public static class UserPath
{
    public static string UserFilePathByFileName(this Paths path, string fileName)
    {
        return $"{path[UserPathType.UsersPath]}/{fileName}".ToLower();
    }

    public static string UserFilePathByUserId(this Paths path, string userId)
    {
        return $"{path[UserPathType.UsersPath]}/user.{userId}.json".ToLower();
    }
}