namespace Tests.Bob;

public class BobRecommendationTest
{
    [Fact]
    public void Analyze_ReturnsMissingIngredients_ExceptBasicIngredients()
    {
        var store = new BobDataStore
        {
            Menus =
            [
                new BobMenu
                {
                    Name = "소고기무국",
                    Ingredients =
                    [
                        Ingredient("소고기", 50, "g"),
                        Ingredient("무", 30, "g"),
                        Ingredient("소금", 1, "작은술", isBasic: true),
                    ],
                },
            ],
            FridgeItems =
            [
                new BobFridgeItem { Name = "소고기", Quantity = 50, Unit = "g" },
            ],
        };

        var result = new BobMenuAvailabilityAnalyzer().Analyze(store);

        result.Should().ContainSingle();
        result[0].IsNearCandidate.Should().BeTrue();
        result[0].MissingIngredients.Should().ContainSingle(x => x.Name == "무");
        result[0].MissingIngredients.Should().NotContain(x => x.Name == "소금");
    }

    [Fact]
    public void Analyze_MarksRestrictedMenu_WhenAllergyIngredientExists()
    {
        var store = new BobDataStore
        {
            ChildProfile = new BobChildProfile
            {
                AllergyIngredients = ["계란"],
            },
            Menus =
            [
                new BobMenu
                {
                    Name = "계란찜",
                    Ingredients = [Ingredient("계란", 1, "개")],
                },
            ],
            FridgeItems =
            [
                new BobFridgeItem { Name = "계란", Quantity = 10, Unit = "개" },
            ],
        };

        var result = new BobMenuAvailabilityAnalyzer().Analyze(store);

        result[0].HasRestrictedIngredient.Should().BeTrue();
        result[0].CanCook.Should().BeFalse();
    }

    [Fact]
    public void CalculateShoppingList_AppliesPurchaseUnit_AndSkipsBasicIngredients()
    {
        var menu = new BobMenu
        {
            Name = "소고기두부덮밥",
            Ingredients =
            [
                Ingredient("소고기", 200, "g"),
                Ingredient("두부", 0.5m, "모"),
                Ingredient("간장", 1, "큰술", isBasic: true),
            ],
        };
        var store = new BobDataStore
        {
            FridgeItems =
            [
                new BobFridgeItem { Name = "소고기", Quantity = 50, Unit = "g" },
            ],
            PurchaseUnits =
            [
                new BobPurchaseUnit
                {
                    IngredientName = "소고기",
                    MinimumQuantity = 300,
                    Unit = "g",
                    Category = BobIngredientCategory.Meat,
                },
            ],
        };

        var shoppingItems = new BobShoppingListCalculator().Calculate(store, [menu]);

        var beef = shoppingItems.Should().ContainSingle(x => x.IngredientName == "소고기").Subject;
        beef.RequiredQuantity.Should().Be(150);
        beef.RecommendedQuantity.Should().Be(300);
        beef.RemainingQuantity.Should().Be(150);
        shoppingItems.Should().NotContain(x => x.IngredientName == "간장");
    }

    [Fact]
    public void CreateWeeklyPlan_DoesNotRecommendRestrictedMenus()
    {
        var restrictedMenu = new BobMenu
        {
            Id = Guid.NewGuid(),
            Name = "계란찜",
            TasteLevel = BobTasteLevel.Favorite,
            Ingredients = [Ingredient("계란", 1, "개")],
        };
        var availableMenu = new BobMenu
        {
            Id = Guid.NewGuid(),
            Name = "두부구이",
            TasteLevel = BobTasteLevel.Good,
            Ingredients = [Ingredient("두부", 0.5m, "모")],
        };
        var store = new BobDataStore
        {
            ChildProfile = new BobChildProfile
            {
                AllergyIngredients = ["계란"],
            },
            Menus = [restrictedMenu, availableMenu],
        };

        var plan = new BobWeeklyMealPlanner().CreatePlan(store, new DateOnly(2026, 5, 18));

        plan.Meals.SelectMany(x => x.MenuIds).Should().NotContain(restrictedMenu.Id);
        plan.Meals.SelectMany(x => x.MenuIds).Should().Contain(availableMenu.Id);
    }

    private static BobMenuIngredient Ingredient(string name, decimal quantity, string unit, bool isBasic = false)
    {
        return new BobMenuIngredient
        {
            Name = name,
            Quantity = quantity,
            Unit = unit,
            IsRequired = true,
            IsBasic = isBasic,
        };
    }
}
