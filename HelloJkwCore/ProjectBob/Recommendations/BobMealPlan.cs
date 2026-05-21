namespace ProjectBob;

public class BobMealPlan
{
    public DateOnly Date { get; set; }
    public BobMealType MealType { get; set; }
    public List<Guid> MenuIds { get; set; } = [];
    public List<BobMenu> Menus { get; set; } = [];

    public string MenuNames => Menus.Count == 0
        ? "추천 없음"
        : string.Join(", ", Menus.Select(menu => menu.Name));
}
