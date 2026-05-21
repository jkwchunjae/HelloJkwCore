namespace ProjectBob;

public static class BobIngredientRules
{
    private static readonly string[] MeatWords = ["소고기", "닭고기", "돼지고기", "생선", "대구", "연어", "고기"];
    private static readonly string[] VegetableWords = ["애호박", "당근", "브로콜리", "양배추", "시금치", "무", "버섯", "양파", "오이", "가지", "파프리카", "토마토"];
    private static readonly string[] DairyWords = ["우유", "치즈", "요거트", "요구르트", "분유"];
    private static readonly string[] GrainWords = ["쌀", "밥", "죽", "면", "국수", "빵", "감자", "고구마", "오트밀"];
    private static readonly string[] FruitWords = ["바나나", "사과", "배", "딸기", "귤", "블루베리", "복숭아", "과일"];
    private static readonly string[] ProteinWords = ["소고기", "닭고기", "돼지고기", "계란", "달걀", "두부", "생선", "대구", "연어", "콩", "우유", "치즈", "요거트"];

    public static string NormalizeName(string value)
        => (value ?? string.Empty).Trim().ToLowerInvariant();

    public static string NormalizeUnit(string value)
        => (value ?? string.Empty).Trim().ToLowerInvariant();

    public static BobIngredientCategory GuessCategory(string ingredientName)
    {
        var name = NormalizeName(ingredientName);

        if (ContainsAny(name, MeatWords))
            return BobIngredientCategory.Meat;
        if (ContainsAny(name, VegetableWords))
            return BobIngredientCategory.Vegetable;
        if (ContainsAny(name, DairyWords))
            return BobIngredientCategory.Dairy;
        if (ContainsAny(name, GrainWords))
            return BobIngredientCategory.Grain;
        if (ContainsAny(name, FruitWords))
            return BobIngredientCategory.Fruit;

        return BobIngredientCategory.Other;
    }

    public static IReadOnlySet<BobNutritionRole> GuessNutritionRoles(BobMenu menu)
    {
        var roles = new HashSet<BobNutritionRole>();

        AddRoles(menu.Name, roles);
        foreach (var ingredient in menu.Ingredients.Where(x => !x.IsBasic))
        {
            AddRoles(ingredient.Name, roles);
        }

        if (menu.Type == BobMenuType.Rice || menu.Type == BobMenuType.Porridge || menu.Type == BobMenuType.Noodle)
            roles.Add(BobNutritionRole.Carbohydrate);
        if (menu.Type == BobMenuType.Fruit || menu.Type == BobMenuType.Snack)
            roles.Add(BobNutritionRole.FruitSnack);

        if (roles.Count == 0)
            roles.Add(BobNutritionRole.Other);

        return roles;
    }

    public static bool IsSameIngredient(string left, string right)
        => NormalizeName(left) == NormalizeName(right);

    private static void AddRoles(string rawName, HashSet<BobNutritionRole> roles)
    {
        var name = NormalizeName(rawName);

        if (ContainsAny(name, GrainWords))
            roles.Add(BobNutritionRole.Carbohydrate);
        if (ContainsAny(name, ProteinWords))
            roles.Add(BobNutritionRole.Protein);
        if (ContainsAny(name, VegetableWords))
            roles.Add(BobNutritionRole.Vegetable);
        if (ContainsAny(name, FruitWords))
            roles.Add(BobNutritionRole.FruitSnack);
    }

    private static bool ContainsAny(string name, IEnumerable<string> words)
        => words.Any(word => name.Contains(word, StringComparison.OrdinalIgnoreCase));
}
