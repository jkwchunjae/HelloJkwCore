namespace ProjectBob;

public class BobMissingIngredient
{
    public string Name { get; set; } = string.Empty;
    public decimal RequiredQuantity { get; set; }
    public decimal AvailableQuantity { get; set; }
    public decimal MissingQuantity => Math.Max(0, RequiredQuantity - AvailableQuantity);
    public string Unit { get; set; } = string.Empty;
    public string? SubstituteName { get; set; }
}
