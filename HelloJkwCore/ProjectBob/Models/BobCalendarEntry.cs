namespace ProjectBob;

public class BobCalendarEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public BobMealType MealType { get; set; } = BobMealType.Breakfast;
    public List<Guid> MenuIds { get; set; } = [];
    public string Memo { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
