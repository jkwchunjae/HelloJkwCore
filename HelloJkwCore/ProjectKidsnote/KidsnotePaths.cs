using Common;

namespace ProjectKidsnote;

public static class KidsnotePathType
{
    public static readonly string ReportPath = nameof(ReportPath);
}

public static class KidsnotePath
{
    public static string Reports(this Paths paths)
    {
        return paths[KidsnotePathType.ReportPath];
    }
}