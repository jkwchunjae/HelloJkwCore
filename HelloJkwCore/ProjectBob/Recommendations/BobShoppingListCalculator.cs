namespace ProjectBob;

public class BobShoppingListCalculator
{
    public List<BobShoppingItem> Calculate(BobDataStore store, IEnumerable<BobMenu> menus)
    {
        var requiredBook = BobIngredientAmountBook.FromMenus(menus);
        var fridgeBook = BobIngredientAmountBook.FromFridge(store.FridgeItems);

        return requiredBook.Items
            .Select(amount => CreateShoppingItem(store, amount, fridgeBook))
            .Where(item => item.RequiredQuantity > 0)
            .OrderBy(item => item.Category)
            .ThenBy(item => item.IngredientName)
            .ToList();
    }

    public BobIngredientAmountBook BuildRemainingBook(BobDataStore store, IEnumerable<BobShoppingItem> shoppingItems, IEnumerable<BobMenu> plannedMenus)
    {
        var remaining = BobIngredientAmountBook.FromFridge(store.FridgeItems);
        foreach (var item in shoppingItems)
        {
            remaining.Add(item.IngredientName, item.RecommendedQuantity, item.Unit);
        }

        foreach (var ingredient in plannedMenus.SelectMany(menu => menu.Ingredients).Where(x => !x.IsBasic))
        {
            remaining.Remove(ingredient.Name, ingredient.Quantity, ingredient.Unit);
        }

        return remaining;
    }

    private static BobShoppingItem CreateShoppingItem(
        BobDataStore store,
        BobIngredientAmountBook.IngredientAmount requiredAmount,
        BobIngredientAmountBook fridgeBook)
    {
        var fridgeQuantity = fridgeBook.GetQuantity(requiredAmount.Name, requiredAmount.Unit);
        var requiredQuantity = Math.Max(0, requiredAmount.Quantity - fridgeQuantity);
        var purchaseUnit = FindPurchaseUnit(store.PurchaseUnits, requiredAmount.Name, requiredAmount.Unit);
        var recommendedQuantity = requiredQuantity;
        var category = BobIngredientRules.GuessCategory(requiredAmount.Name);

        if (purchaseUnit != null)
        {
            // 최소 구매 단위보다 필요량이 작으면 실제 장보기 권장량은 최소 구매 단위로 올린다.
            recommendedQuantity = Math.Max(requiredQuantity, purchaseUnit.MinimumQuantity);
            category = purchaseUnit.Category;
        }

        return new BobShoppingItem
        {
            IngredientName = requiredAmount.Name,
            RequiredQuantity = requiredQuantity,
            RecommendedQuantity = recommendedQuantity,
            Unit = requiredAmount.Unit,
            Category = category,
        };
    }

    private static BobPurchaseUnit? FindPurchaseUnit(IEnumerable<BobPurchaseUnit> purchaseUnits, string name, string unit)
    {
        return purchaseUnits.FirstOrDefault(unitInfo =>
            BobIngredientRules.IsSameIngredient(unitInfo.IngredientName, name)
            && BobIngredientRules.NormalizeUnit(unitInfo.Unit) == BobIngredientRules.NormalizeUnit(unit));
    }
}
