namespace ProjectBob.Pages;

public partial class BobRecommendationPanel
{
    [Inject] public IBobService BobService { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;

    private bool _loading = true;
    private List<BobMenuAvailability> _availability = [];
    private BobWeeklyPlan? _weeklyPlan;
    private DateTime? _weekStartDate = GetCurrentWeekStart().ToDateTime(TimeOnly.MinValue);

    protected override async Task OnInitializedAsync()
    {
        await RefreshAvailabilityAsync();
    }

    private async Task RefreshAvailabilityAsync()
    {
        _loading = true;
        _availability = await BobService.FindMenusFromFridgeAsync();
        _loading = false;
    }

    private async Task CreateWeeklyPlanAsync()
    {
        var weekStart = DateOnly.FromDateTime(_weekStartDate ?? DateTime.Today);
        _weeklyPlan = await BobService.CreateWeeklyPlanAsync(weekStart);
        Snackbar.Add("주간 식단 추천을 생성했습니다.", Severity.Success);
    }

    private async Task ConfirmWeeklyPlanAsync()
    {
        if (_weeklyPlan == null)
            return;

        await BobService.ConfirmWeeklyPlanAsync(_weeklyPlan);
        Snackbar.Add("주간 식단을 캘린더에 반영했습니다.", Severity.Success);
        await RefreshAvailabilityAsync();
    }

    private static string GetAvailabilityText(BobMenuAvailability item)
    {
        if (item.HasRestrictedIngredient)
            return "제한";
        if (item.CanCook)
            return "가능";
        if (item.IsNearCandidate)
            return "부족 1~2개";
        return "재료 부족";
    }

    private static Color GetAvailabilityColor(BobMenuAvailability item)
    {
        if (item.HasRestrictedIngredient)
            return Color.Error;
        if (item.CanCook)
            return Color.Success;
        if (item.IsNearCandidate)
            return Color.Warning;
        return Color.Default;
    }

    private static string FormatMissingIngredients(BobMenuAvailability item)
    {
        if (item.HasRestrictedIngredient)
            return "제한 재료 포함";
        if (item.MissingIngredients.Count == 0)
            return "-";

        return string.Join(", ", item.MissingIngredients.Select(x => $"{x.Name} {x.MissingQuantity}{x.Unit}"));
    }

    private static string FormatLastServed(DateOnly? date)
    {
        return date?.ToString("yyyy.MM.dd") ?? "-";
    }

    private static DateOnly GetCurrentWeekStart()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var offset = ((int)today.DayOfWeek + 6) % 7;
        return today.AddDays(-offset);
    }
}
