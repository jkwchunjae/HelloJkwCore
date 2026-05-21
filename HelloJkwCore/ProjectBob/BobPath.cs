namespace ProjectBob;

public static class BobPathType
{
    public static readonly string StoreFile = nameof(StoreFile);
}

public static class BobPath
{
    public static string StoreFile(this Paths paths)
    {
        return paths[BobPathType.StoreFile];
    }
}
