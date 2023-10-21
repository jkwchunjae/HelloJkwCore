using GameLibra.Service;

namespace GameLibra.Page;

public partial class LibraAssistComponent : JkwPageBase
{
    [Parameter] public IReadOnlyList<IReadOnlyDictionary<string, int>> AssistSets { get; set; }
    IReadOnlyList<IReadOnlyDictionary<string, int>> Sets
    {
        get
        {
            if (AssistSets.Count() < 1000)
            {
                return AssistSets;
            }
            else
            {
                return null;
            }
        }
    }
    List<string> CubeNames => Sets.First()
        .OrderBy(x => x.Value)
        .Select(x => x.Key)
        .ToList();

    protected override Task OnPageInitializedAsync()
    {
        return base.OnPageInitializedAsync();
    }
}