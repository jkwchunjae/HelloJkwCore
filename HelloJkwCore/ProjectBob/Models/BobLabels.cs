namespace ProjectBob;

public static class BobLabels
{
    public static string Label(this BobMenuType type)
        => type switch
        {
            BobMenuType.Rice => "밥",
            BobMenuType.Soup => "국",
            BobMenuType.SideDish => "반찬",
            BobMenuType.Snack => "간식",
            BobMenuType.Porridge => "죽",
            BobMenuType.Noodle => "면",
            BobMenuType.Fruit => "과일",
            _ => "기타",
        };

    public static string Label(this BobMealType type)
        => type switch
        {
            BobMealType.Breakfast => "아침",
            BobMealType.Lunch => "점심",
            BobMealType.Dinner => "저녁",
            BobMealType.Snack => "간식",
            _ => "끼니",
        };

    public static string Label(this BobTasteLevel level)
        => level switch
        {
            BobTasteLevel.Low => "잘 안 먹음",
            BobTasteLevel.Normal => "보통",
            BobTasteLevel.Good => "잘 먹음",
            BobTasteLevel.Favorite => "매우 잘 먹음",
            _ => "미정",
        };

    public static string Label(this BobCookingDifficulty difficulty)
        => difficulty switch
        {
            BobCookingDifficulty.Easy => "쉬움",
            BobCookingDifficulty.Normal => "보통",
            BobCookingDifficulty.Hard => "어려움",
            _ => "보통",
        };

    public static string Label(this BobIngredientCategory category)
        => category switch
        {
            BobIngredientCategory.Meat => "육류",
            BobIngredientCategory.Vegetable => "채소",
            BobIngredientCategory.Dairy => "유제품",
            BobIngredientCategory.Grain => "곡류",
            BobIngredientCategory.Fruit => "과일",
            _ => "기타",
        };

    public static string Label(this BobNutritionRole role)
        => role switch
        {
            BobNutritionRole.Carbohydrate => "탄수화물",
            BobNutritionRole.Protein => "단백질",
            BobNutritionRole.Vegetable => "채소",
            BobNutritionRole.FruitSnack => "과일/간식",
            _ => "기타",
        };
}
