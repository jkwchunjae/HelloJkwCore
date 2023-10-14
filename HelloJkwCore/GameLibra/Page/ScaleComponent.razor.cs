
namespace GameLibra.Page;

public partial class ScaleComponent : JkwPageBase
{
    [Parameter] public DoubleScale Scale { get; set; }
    [Parameter] public Player CurrentPlayer { get; set; }
    [Parameter] public LibraBoardSetting Setting { get; set; }

    private string ScaleTitle => Scale.Id == 1 ? "메인 저울" : "보조 저울";
    private string LeftArmCubeIdentifier => $"scale-{Scale.Id}-left";
    private string RightArmCubeIdentifier => $"scale-{Scale.Id}-right";
    private string LargeContainerClass => Setting.LargeContainer ? "large-container" : "";
    private string LargeSlotClass => Setting.LargeContainer ? "large-slot" : "";
    private string OrderedContainerClass => Setting.OrderedConatiner ? "ordered-container" : "";
}
