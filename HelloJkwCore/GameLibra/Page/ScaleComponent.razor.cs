
namespace GameLibra.Page;

public partial class ScaleComponent : JkwPageBase
{
    [Parameter]
    public DoubleScale Scale { get; set; }

    private string LeftArmCubeIdentifier => $"scale-{Scale.Id}-left";
    private string RightArmCubeIdentifier => $"scale-{Scale.Id}-right";

    int[] ids => Scale.Left.Cubes.Select(x => x.Id)
        .Concat(Scale.Right.Cubes.Select(x => x.Id))
        .Distinct()
        .ToArray();
}
