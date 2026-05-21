namespace ProjectBob.Pages;

public partial class BobChildProfilePanel
{
    [Inject] public IBobService BobService { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;

    private bool _loading = true;
    private string _allergyText = string.Empty;
    private string _blockedText = string.Empty;
    private string _dislikedText = string.Empty;
    private string _favoriteMenuText = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        var store = await BobService.GetStoreAsync();
        _allergyText = JoinCsv(store.ChildProfile.AllergyIngredients);
        _blockedText = JoinCsv(store.ChildProfile.BlockedIngredients);
        _dislikedText = JoinCsv(store.ChildProfile.DislikedIngredients);
        _favoriteMenuText = JoinCsv(store.ChildProfile.FavoriteMenuNames);
        _loading = false;
    }

    private async Task SaveAsync()
    {
        var profile = new BobChildProfile
        {
            AllergyIngredients = SplitCsv(_allergyText),
            BlockedIngredients = SplitCsv(_blockedText),
            DislikedIngredients = SplitCsv(_dislikedText),
            FavoriteMenuNames = SplitCsv(_favoriteMenuText),
        };

        await BobService.SaveChildProfileAsync(profile);
        Snackbar.Add("아이 제한 정보를 저장했습니다.", Severity.Success);
    }

    private static string JoinCsv(IEnumerable<string> values)
    {
        return string.Join(", ", values);
    }

    private static List<string> SplitCsv(string text)
    {
        return text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
