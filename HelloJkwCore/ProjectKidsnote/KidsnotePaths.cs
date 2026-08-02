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

    public static string KidsReportFolder(this Paths paths, long childId)
    {
        return $"{paths.Reports()}/{childId}";
    }

    public static string KidsReportRootFile(this Paths paths, long childId)
    {
        return $"{paths.KidsReportFolder(childId)}/_index.json";
    }

    public static string KidsReportFile(
        this Paths paths,
        long childId,
        long reportId)
    {
        return $"{paths.KidsReportFolder(childId)}/{reportId}.json";
    }
}
