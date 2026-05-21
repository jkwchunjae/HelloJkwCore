namespace ProjectBob;

public class BobWeeklyMealPlanner
{
    private readonly BobShoppingListCalculator _shoppingListCalculator = new();
    private readonly BobMenuAvailabilityAnalyzer _availabilityAnalyzer = new();

    public BobWeeklyPlan CreatePlan(BobDataStore store, DateOnly weekStart)
    {
        var menus = store.Menus
            .Where(menu => menu.HasName)
            .Where(menu => !HasRestrictedIngredient(menu, store.ChildProfile))
            .ToList();

        var usageCount = new Dictionary<Guid, int>();
        var meals = BuildMealSlots(weekStart)
            .Select(slot => CreateMealPlan(slot.Date, slot.MealType, menus, store, usageCount))
            .ToList();

        var plannedMenus = meals.SelectMany(meal => meal.Menus).ToList();
        var shoppingItems = _shoppingListCalculator.Calculate(store, plannedMenus);
        var remainingBook = _shoppingListCalculator.BuildRemainingBook(store, shoppingItems, plannedMenus);
        var remainingFridge = remainingBook.Items.Select(x => new BobFridgeItem
        {
            Name = x.Name,
            Quantity = x.Quantity,
            Unit = x.Unit,
            Category = BobIngredientRules.GuessCategory(x.Name),
        });

        return new BobWeeklyPlan
        {
            WeekStart = weekStart,
            Meals = meals,
            ShoppingItems = shoppingItems,
            ExtraMenuCandidates = _availabilityAnalyzer
                .Analyze(store, remainingFridge)
                .Where(x => x.CanCook)
                .Take(5)
                .ToList(),
        };
    }

    private static BobMealPlan CreateMealPlan(
        DateOnly date,
        BobMealType mealType,
        IReadOnlyList<BobMenu> menus,
        BobDataStore store,
        Dictionary<Guid, int> usageCount)
    {
        var targetCount = GetTargetMenuCount(date, mealType);
        var selected = new List<BobMenu>();
        var coveredRoles = new HashSet<BobNutritionRole>();

        foreach (var menu in ScoreMenus(menus, store, mealType, usageCount))
        {
            if (selected.Count >= targetCount)
                break;

            var roles = BobIngredientRules.GuessNutritionRoles(menu);
            var improvesBalance = roles.Any(role => !coveredRoles.Contains(role));
            var usage = usageCount.GetValueOrDefault(menu.Id);

            if (selected.Count == 0 || improvesBalance || usage == 0)
            {
                selected.Add(menu);
                foreach (var role in roles)
                {
                    coveredRoles.Add(role);
                }

                usageCount[menu.Id] = usage + 1;
            }
        }

        return new BobMealPlan
        {
            Date = date,
            MealType = mealType,
            Menus = selected,
            MenuIds = selected.Select(menu => menu.Id).ToList(),
        };
    }

    private static IEnumerable<BobMenu> ScoreMenus(
        IEnumerable<BobMenu> menus,
        BobDataStore store,
        BobMealType mealType,
        IReadOnlyDictionary<Guid, int> usageCount)
    {
        return menus
            .Select(menu => new
            {
                Menu = menu,
                Score = ScoreMenu(menu, store, mealType, usageCount.GetValueOrDefault(menu.Id)),
            })
            .OrderByDescending(x => x.Score)
            .ThenBy(x => x.Menu.Name)
            .Select(x => x.Menu);
    }

    private static int ScoreMenu(BobMenu menu, BobDataStore store, BobMealType mealType, int usageCount)
    {
        var score = 0;
        score += (int)menu.TasteLevel * 8;
        score += store.ChildProfile.FavoriteMenuNames.Any(name => BobIngredientRules.IsSameIngredient(name, menu.Name)) ? 12 : 0;
        score += menu.CanMakeAhead ? 2 : 0;
        score -= usageCount * 16;

        if (HasDislikedIngredient(menu, store.ChildProfile))
            score -= 20;

        if (mealType == BobMealType.Breakfast)
        {
            score += menu.IsGoodForBreakfast ? 20 : 0;
            score += menu.CookingMinutes <= 15 ? 8 : -8;
            score += menu.Difficulty == BobCookingDifficulty.Easy ? 6 : 0;
        }
        else if (mealType == BobMealType.Dinner)
        {
            score += BobIngredientRules.GuessNutritionRoles(menu).Contains(BobNutritionRole.Protein) ? 8 : 0;
            score += BobIngredientRules.GuessNutritionRoles(menu).Contains(BobNutritionRole.Vegetable) ? 8 : 0;
        }
        else if (mealType == BobMealType.Lunch)
        {
            score += menu.CookingMinutes <= 40 ? 4 : 0;
        }

        return score;
    }

    private static IEnumerable<(DateOnly Date, BobMealType MealType)> BuildMealSlots(DateOnly weekStart)
    {
        for (var dayOffset = 0; dayOffset < 7; dayOffset++)
        {
            var date = weekStart.AddDays(dayOffset);
            yield return (date, BobMealType.Breakfast);

            if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                yield return (date, BobMealType.Lunch);

            yield return (date, BobMealType.Dinner);
        }
    }

    private static int GetTargetMenuCount(DateOnly date, BobMealType mealType)
    {
        return mealType switch
        {
            BobMealType.Breakfast => 2,
            BobMealType.Lunch when date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday => 2,
            BobMealType.Dinner => 3,
            _ => 1,
        };
    }

    private static bool HasRestrictedIngredient(BobMenu menu, BobChildProfile profile)
    {
        return menu.Ingredients.Any(ingredient =>
            profile.AllergyIngredients.Any(allergy => BobIngredientRules.IsSameIngredient(allergy, ingredient.Name))
            || profile.BlockedIngredients.Any(blocked => BobIngredientRules.IsSameIngredient(blocked, ingredient.Name)));
    }

    private static bool HasDislikedIngredient(BobMenu menu, BobChildProfile profile)
    {
        return menu.Ingredients.Any(ingredient =>
            profile.DislikedIngredients.Any(disliked => BobIngredientRules.IsSameIngredient(disliked, ingredient.Name)));
    }
}
