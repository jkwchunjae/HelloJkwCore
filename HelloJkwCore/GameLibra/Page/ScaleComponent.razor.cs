
namespace GameLibra.Page;

public partial class ScaleComponent : JkwPageBase
{
    LibraGameState state;
    DoubleScale scale;
    int[] ids => scale.Left.Cubes.Select(x => x.Id)
        .Concat(scale.Right.Cubes.Select(x => x.Id))
        .Distinct()
        .ToArray();
    protected override void OnPageInitialized()
    {
        state = new GameStateBuilder()
            .UseDevilsPlanRule()
            .Build();
        scale = state.Scales[0];
        scale.Left.Add(state.CubeInfo[0]);
        scale.Left.Add(state.CubeInfo[0]);
        scale.Left.Add(state.CubeInfo[0]);
        scale.Left.Add(state.CubeInfo[0]);
        scale.Right.Add(state.CubeInfo[2]);
    }
}
