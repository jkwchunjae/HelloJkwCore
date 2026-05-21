namespace ProjectBob.Pages;

public partial class BobFridgePanel
{
    [Inject] public IBobService BobService { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;

    private bool _loading = true;
    private List<BobFridgeItem> _fridgeItems = [];
    private List<BobPurchaseUnit> _purchaseUnits = [];
    private BobFridgeItem _newFridgeItem = new();
    private BobPurchaseUnit _editingPurchaseUnit = CreateEmptyPurchaseUnit();

    protected override async Task OnInitializedAsync()
    {
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        _loading = true;
        var store = await BobService.GetStoreAsync();
        _fridgeItems = store.FridgeItems.Select(CloneFridgeItem).OrderBy(x => x.Name).ToList();
        _purchaseUnits = store.PurchaseUnits.Select(ClonePurchaseUnit).OrderBy(x => x.IngredientName).ToList();
        _loading = false;
    }

    private void AddFridgeItem()
    {
        if (string.IsNullOrWhiteSpace(_newFridgeItem.Name) || _newFridgeItem.Quantity <= 0)
        {
            Snackbar.Add("재료명과 수량을 입력하세요.", Severity.Warning);
            return;
        }

        _newFridgeItem.Id = Guid.NewGuid();
        _newFridgeItem.Category = BobIngredientRules.GuessCategory(_newFridgeItem.Name);
        _fridgeItems.Add(CloneFridgeItem(_newFridgeItem));
        _fridgeItems = _fridgeItems.OrderBy(x => x.Name).ToList();
        _newFridgeItem = new BobFridgeItem();
    }

    private void RemoveFridgeItem(Guid itemId)
    {
        _fridgeItems.RemoveAll(item => item.Id == itemId);
    }

    private async Task SaveFridgeAsync()
    {
        await BobService.SaveFridgeItemsAsync(_fridgeItems);
        Snackbar.Add("냉장고 재료를 저장했습니다.", Severity.Success);
        await LoadAsync();
    }

    private async Task SavePurchaseUnitAsync()
    {
        if (string.IsNullOrWhiteSpace(_editingPurchaseUnit.IngredientName) || _editingPurchaseUnit.MinimumQuantity <= 0)
        {
            Snackbar.Add("재료명과 최소 구매 수량을 입력하세요.", Severity.Warning);
            return;
        }

        await BobService.SavePurchaseUnitAsync(ClonePurchaseUnit(_editingPurchaseUnit));
        Snackbar.Add("최소 구매 단위를 저장했습니다.", Severity.Success);
        ResetPurchaseUnit();
        await LoadAsync();
    }

    private async Task DeletePurchaseUnitAsync(Guid purchaseUnitId)
    {
        await BobService.DeletePurchaseUnitAsync(purchaseUnitId);
        Snackbar.Add("최소 구매 단위를 삭제했습니다.", Severity.Info);
        if (_editingPurchaseUnit.Id == purchaseUnitId)
            ResetPurchaseUnit();
        await LoadAsync();
    }

    private void EditPurchaseUnit(BobPurchaseUnit purchaseUnit)
    {
        _editingPurchaseUnit = ClonePurchaseUnit(purchaseUnit);
    }

    private void ResetPurchaseUnit()
    {
        _editingPurchaseUnit = CreateEmptyPurchaseUnit();
    }

    private static BobPurchaseUnit CreateEmptyPurchaseUnit()
    {
        return new BobPurchaseUnit
        {
            Id = Guid.Empty,
            Category = BobIngredientCategory.Other,
        };
    }

    private static BobFridgeItem CloneFridgeItem(BobFridgeItem item)
    {
        return new BobFridgeItem
        {
            Id = item.Id == Guid.Empty ? Guid.NewGuid() : item.Id,
            Name = item.Name,
            Quantity = item.Quantity,
            Unit = item.Unit,
            Category = item.Category,
            UpdatedAt = item.UpdatedAt,
        };
    }

    private static BobPurchaseUnit ClonePurchaseUnit(BobPurchaseUnit purchaseUnit)
    {
        return new BobPurchaseUnit
        {
            Id = purchaseUnit.Id,
            IngredientName = purchaseUnit.IngredientName,
            MinimumQuantity = purchaseUnit.MinimumQuantity,
            Unit = purchaseUnit.Unit,
            Category = purchaseUnit.Category,
            UpdatedAt = purchaseUnit.UpdatedAt,
        };
    }
}
