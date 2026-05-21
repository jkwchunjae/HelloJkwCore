namespace ProjectBob;

public class BobShoppingItem
{
    public string IngredientName { get; set; } = string.Empty;
    public BobIngredientCategory Category { get; set; } = BobIngredientCategory.Other;
    public decimal RequiredQuantity { get; set; }
    public decimal RecommendedQuantity { get; set; }
    public decimal RemainingQuantity => Math.Max(0, RecommendedQuantity - RequiredQuantity);
    public string Unit { get; set; } = string.Empty;
}
