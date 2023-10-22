using GameLibra.Service;

namespace GameLibra.Page;

public partial class LibraAssistComponent : JkwPageBase
{
    [Inject] public ISnackbar Snackbar { get; set; }
    [Parameter] public LibraGameState State { get; set; }
    [Parameter] public IReadOnlyList<IReadOnlyDictionary<string, int>> AssistSets { get; set; }
    IReadOnlyList<IReadOnlyDictionary<string, int>> Sets
    {
        get
        {
            if (AssistSets?.Count() < 1000)
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

    string calcFormula = string.Empty;
    Dictionary<IReadOnlyDictionary<string, int>, object> calcResult = new();

    protected override Task OnPageInitializedAsync()
    {
        return base.OnPageInitializedAsync();
    }

    private Task FormulaChagned(string formula)
    {
        // formula: abc -> int
        // formula: a+b+c -> int
        // formula: ab < ee -> boolean
        // formula: aa = bb -> boolean
        calcResult = new();
        calcFormula = formula;

        if (string.IsNullOrWhiteSpace(formula))
        {
            return Task.CompletedTask;
        }

        try
        {
            var (testResult, errorMessage) = new CubeCalculator().TestValidFormula(formula, CubeNames);
            if (!testResult)
            {
                Snackbar.Add(errorMessage, Severity.Error, options =>
                {
                    options.VisibleStateDuration = 3000;
                });
                return Task.CompletedTask;
            }

            Sets?.ForEach(set =>
            {
                var result = new CubeCalculator().CalculateFormula(formula, set);
                calcResult[set] = result;
            });
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error, options =>
            {
                options.VisibleStateDuration = 3000;
            });
        }

        return Task.CompletedTask;
    }
}