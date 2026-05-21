namespace ProjectBob;

[TextJsonConverter(typeof(TextJsonStringEnumConverter))]
public enum BobMenuType
{
    Rice,
    Soup,
    SideDish,
    Snack,
    Porridge,
    Noodle,
    Fruit,
    Other,
}

[TextJsonConverter(typeof(TextJsonStringEnumConverter))]
public enum BobMealType
{
    Breakfast,
    Lunch,
    Dinner,
    Snack,
}

[TextJsonConverter(typeof(TextJsonStringEnumConverter))]
public enum BobTasteLevel
{
    Unknown = 0,
    Low = 1,
    Normal = 2,
    Good = 3,
    Favorite = 4,
}

[TextJsonConverter(typeof(TextJsonStringEnumConverter))]
public enum BobCookingDifficulty
{
    Easy,
    Normal,
    Hard,
}

[TextJsonConverter(typeof(TextJsonStringEnumConverter))]
public enum BobIngredientCategory
{
    Meat,
    Vegetable,
    Dairy,
    Grain,
    Fruit,
    Other,
}

[TextJsonConverter(typeof(TextJsonStringEnumConverter))]
public enum BobNutritionRole
{
    Carbohydrate,
    Protein,
    Vegetable,
    FruitSnack,
    Other,
}
