namespace ProjectBob.Pages;

public partial class BobCalendarPanel
{
    [Inject] public IBobService BobService { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;

    private bool _loading = true;
    private List<BobMenu> _menus = [];
    private List<BobCalendarEntry> _recentEntries = [];
    private DateTime? _selectedDate = DateTime.Today;
    private BobMealType _selectedMealType = BobMealType.Breakfast;
    private HashSet<Guid> _selectedMenuIds = [];
    private Guid _editingEntryId = Guid.Empty;
    private string _memo = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadAsync();
        LoadSelectedEntry();
    }

    private async Task LoadAsync()
    {
        _loading = true;
        var store = await BobService.GetStoreAsync();
        _menus = store.Menus.OrderBy(menu => menu.Name).ToList();
        _recentEntries = store.CalendarEntries
            .OrderByDescending(entry => entry.Date)
            .ThenBy(entry => entry.MealType)
            .Take(30)
            .ToList();
        _loading = false;
    }

    private void LoadSelectedEntry()
    {
        var date = DateOnly.FromDateTime(_selectedDate ?? DateTime.Today);
        var entry = _recentEntries.FirstOrDefault(x => x.Date == date && x.MealType == _selectedMealType);

        if (entry == null)
        {
            _editingEntryId = Guid.Empty;
            _selectedMenuIds = [];
            _memo = string.Empty;
            return;
        }

        EditEntry(entry);
    }

    private async Task SaveEntryAsync()
    {
        if (_selectedMenuIds.Count == 0 && string.IsNullOrWhiteSpace(_memo))
        {
            Snackbar.Add("메뉴 또는 메모를 입력하세요.", Severity.Warning);
            return;
        }

        var entry = new BobCalendarEntry
        {
            Id = _editingEntryId,
            Date = DateOnly.FromDateTime(_selectedDate ?? DateTime.Today),
            MealType = _selectedMealType,
            MenuIds = _selectedMenuIds.ToList(),
            Memo = _memo,
        };

        await BobService.SaveCalendarEntryAsync(entry);
        Snackbar.Add("식단 기록을 저장했습니다.", Severity.Success);
        await LoadAsync();
        LoadSelectedEntry();
    }

    private async Task DeleteEntryAsync()
    {
        if (_editingEntryId == Guid.Empty)
            return;

        await BobService.DeleteCalendarEntryAsync(_editingEntryId);
        Snackbar.Add("식단 기록을 삭제했습니다.", Severity.Info);
        _editingEntryId = Guid.Empty;
        _selectedMenuIds = [];
        _memo = string.Empty;
        await LoadAsync();
    }

    private void EditEntry(BobCalendarEntry entry)
    {
        _editingEntryId = entry.Id;
        _selectedDate = entry.Date.ToDateTime(TimeOnly.MinValue);
        _selectedMealType = entry.MealType;
        _selectedMenuIds = entry.MenuIds.ToHashSet();
        _memo = entry.Memo;
    }

    private bool IsMenuSelected(Guid menuId)
    {
        return _selectedMenuIds.Contains(menuId);
    }

    private void ToggleMenu(Guid menuId, bool selected)
    {
        if (selected)
            _selectedMenuIds.Add(menuId);
        else
            _selectedMenuIds.Remove(menuId);
    }

    private string FormatEntryMenus(BobCalendarEntry entry)
    {
        var menuNames = entry.MenuIds
            .Select(id => _menus.FirstOrDefault(menu => menu.Id == id)?.Name)
            .Where(name => !string.IsNullOrWhiteSpace(name));

        return string.Join(", ", menuNames);
    }
}
