namespace ProjectBob;

public class BobWeeklyPlan
{
    public DateOnly WeekStart { get; set; }
    public List<BobMealPlan> Meals { get; set; } = [];
    public List<BobShoppingItem> ShoppingItems { get; set; } = [];
    public List<BobMenuAvailability> ExtraMenuCandidates { get; set; } = [];
}
