namespace ProjectBob;

public class BobPurchaseUnit
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string IngredientName { get; set; } = string.Empty;
    public decimal MinimumQuantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public BobIngredientCategory Category { get; set; } = BobIngredientCategory.Other;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
