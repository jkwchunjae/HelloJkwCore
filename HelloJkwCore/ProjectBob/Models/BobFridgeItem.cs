namespace ProjectBob;

public class BobFridgeItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public BobIngredientCategory Category { get; set; } = BobIngredientCategory.Other;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
