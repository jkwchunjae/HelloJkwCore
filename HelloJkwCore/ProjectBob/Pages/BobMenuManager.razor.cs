namespace ProjectBob.Pages;

public partial class BobMenuManager
{
    [Inject] public IBobService BobService { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;

    private bool _loading = true;
    private List<BobMenu> _menus = [];
    private BobMenu _editingMenu = CreateEmptyMenu();
    private BobMenuIngredient _newIngredient = CreateEmptyIngredient();
    private string _newSubstitutes = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        _loading = true;
        var store = await BobService.GetStoreAsync();
        _menus = store.Menus.OrderBy(menu => menu.Name).ToList();
        _loading = false;
    }

    private async Task SaveMenuAsync()
    {
        if (string.IsNullOrWhiteSpace(_editingMenu.Name))
        {
            Snackbar.Add("메뉴 이름을 입력하세요.", Severity.Warning);
            return;
        }

        if (_editingMenu.Ingredients.Count == 0)
        {
            Snackbar.Add("재료를 1개 이상 등록하세요.", Severity.Warning);
            return;
        }

        await BobService.SaveMenuAsync(CloneMenu(_editingMenu));
        Snackbar.Add("메뉴를 저장했습니다.", Severity.Success);
        ResetEditor();
        await LoadAsync();
    }

    private async Task DeleteMenuAsync(Guid menuId)
    {
        await BobService.DeleteMenuAsync(menuId);
        Snackbar.Add("메뉴를 삭제했습니다.", Severity.Info);
        if (_editingMenu.Id == menuId)
            ResetEditor();
        await LoadAsync();
    }

    private void EditMenu(BobMenu menu)
    {
        _editingMenu = CloneMenu(menu);
        _newIngredient = CreateEmptyIngredient();
        _newSubstitutes = string.Empty;
    }

    private void ResetEditor()
    {
        _editingMenu = CreateEmptyMenu();
        _newIngredient = CreateEmptyIngredient();
        _newSubstitutes = string.Empty;
    }

    private void AddIngredient()
    {
        if (string.IsNullOrWhiteSpace(_newIngredient.Name) || _newIngredient.Quantity <= 0)
        {
            Snackbar.Add("재료명과 수량을 입력하세요.", Severity.Warning);
            return;
        }

        var ingredient = CloneIngredient(_newIngredient);
        ingredient.SubstituteNames = SplitCsv(_newSubstitutes);
        _editingMenu.Ingredients.Add(ingredient);

        _newIngredient = CreateEmptyIngredient();
        _newSubstitutes = string.Empty;
    }

    private void RemoveIngredient(Guid ingredientId)
    {
        _editingMenu.Ingredients.RemoveAll(ingredient => ingredient.Id == ingredientId);
    }

    private static string FormatIngredientFlags(BobMenuIngredient ingredient)
    {
        var flags = new List<string>();
        if (ingredient.IsRequired)
            flags.Add("필수");
        if (ingredient.IsBasic)
            flags.Add("기본");
        if (ingredient.SubstituteNames.Count > 0)
            flags.Add($"대체: {string.Join(", ", ingredient.SubstituteNames)}");

        return flags.Count == 0 ? "-" : string.Join(" / ", flags);
    }

    private static BobMenu CreateEmptyMenu()
    {
        return new BobMenu
        {
            Id = Guid.Empty,
            Type = BobMenuType.SideDish,
            TasteLevel = BobTasteLevel.Normal,
            Difficulty = BobCookingDifficulty.Normal,
            CookingMinutes = 20,
        };
    }

    private static BobMenuIngredient CreateEmptyIngredient()
    {
        return new BobMenuIngredient
        {
            Id = Guid.NewGuid(),
            IsRequired = true,
        };
    }

    private static BobMenu CloneMenu(BobMenu menu)
    {
        return new BobMenu
        {
            Id = menu.Id,
            Name = menu.Name,
            Type = menu.Type,
            Ingredients = menu.Ingredients.Select(CloneIngredient).ToList(),
            TasteLevel = menu.TasteLevel,
            Difficulty = menu.Difficulty,
            CookingMinutes = menu.CookingMinutes,
            IsGoodForBreakfast = menu.IsGoodForBreakfast,
            CanMakeAhead = menu.CanMakeAhead,
            Memo = menu.Memo,
            CreatedAt = menu.CreatedAt,
            UpdatedAt = menu.UpdatedAt,
        };
    }

    private static BobMenuIngredient CloneIngredient(BobMenuIngredient ingredient)
    {
        return new BobMenuIngredient
        {
            Id = ingredient.Id == Guid.Empty ? Guid.NewGuid() : ingredient.Id,
            Name = ingredient.Name,
            Quantity = ingredient.Quantity,
            Unit = ingredient.Unit,
            IsRequired = ingredient.IsRequired,
            IsBasic = ingredient.IsBasic,
            SubstituteNames = ingredient.SubstituteNames.ToList(),
        };
    }

    private static List<string> SplitCsv(string text)
    {
        return text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
