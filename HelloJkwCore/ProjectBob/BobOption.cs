namespace ProjectBob;

public class BobOption
{
    public FileSystemSelectOption FileSystemSelect { get; set; } = new() { UseMainFileSystem = true };
    public PathMap Path { get; set; } = new();
}
