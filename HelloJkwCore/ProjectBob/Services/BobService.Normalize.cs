namespace ProjectBob;

public partial class BobService
{
    private static void NormalizeStore(BobDataStore store)
    {
        store.Menus ??= [];
        store.FridgeItems ??= [];
        store.PurchaseUnits ??= [];
        store.ChildProfile ??= new BobChildProfile();
        store.CalendarEntries ??= [];

        foreach (var menu in store.Menus)
        {
            NormalizeMenu(menu);
        }

        store.FridgeItems = store.FridgeItems.Select(NormalizeFridgeItem).ToList();
        foreach (var purchaseUnit in store.PurchaseUnits)
        {
            NormalizePurchaseUnit(purchaseUnit);
        }

        store.ChildProfile = NormalizeProfile(store.ChildProfile);
    }

    private static void NormalizeMenu(BobMenu menu)
    {
        menu.Id = menu.Id == Guid.Empty ? Guid.NewGuid() : menu.Id;
        menu.Name = menu.Name.Trim();
        menu.Memo = menu.Memo?.Trim() ?? string.Empty;
        menu.Ingredients ??= [];

        foreach (var ingredient in menu.Ingredients)
        {
            ingredient.Id = ingredient.Id == Guid.Empty ? Guid.NewGuid() : ingredient.Id;
            ingredient.Name = ingredient.Name.Trim();
            ingredient.Unit = ingredient.Unit.Trim();
            ingredient.SubstituteNames = ingredient.SubstituteNames
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Select(name => name.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        // 빈 재료가 저장소에 남으면 추천/장보기 합산이 흔들리므로 저장 직전에 제거한다.
        menu.Ingredients = menu.Ingredients
            .Where(ingredient => !string.IsNullOrWhiteSpace(ingredient.Name) && ingredient.Quantity > 0)
            .ToList();
    }

    private static BobFridgeItem NormalizeFridgeItem(BobFridgeItem item)
    {
        item.Id = item.Id == Guid.Empty ? Guid.NewGuid() : item.Id;
        item.Name = item.Name.Trim();
        item.Unit = item.Unit.Trim();
        item.Category = item.Category == BobIngredientCategory.Other
            ? BobIngredientRules.GuessCategory(item.Name)
            : item.Category;
        item.UpdatedAt = DateTime.Now;
        return item;
    }

    private static void NormalizePurchaseUnit(BobPurchaseUnit purchaseUnit)
    {
        purchaseUnit.Id = purchaseUnit.Id == Guid.Empty ? Guid.NewGuid() : purchaseUnit.Id;
        purchaseUnit.IngredientName = purchaseUnit.IngredientName.Trim();
        purchaseUnit.Unit = purchaseUnit.Unit.Trim();
        purchaseUnit.Category = purchaseUnit.Category == BobIngredientCategory.Other
            ? BobIngredientRules.GuessCategory(purchaseUnit.IngredientName)
            : purchaseUnit.Category;
    }

    private static BobChildProfile NormalizeProfile(BobChildProfile profile)
    {
        profile.AllergyIngredients = NormalizeStringList(profile.AllergyIngredients);
        profile.DislikedIngredients = NormalizeStringList(profile.DislikedIngredients);
        profile.BlockedIngredients = NormalizeStringList(profile.BlockedIngredients);
        profile.FavoriteMenuNames = NormalizeStringList(profile.FavoriteMenuNames);
        return profile;
    }

    private static List<string> NormalizeStringList(IEnumerable<string>? values)
    {
        return values?
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList() ?? [];
    }
}
