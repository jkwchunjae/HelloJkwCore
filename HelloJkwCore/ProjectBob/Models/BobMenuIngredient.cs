namespace ProjectBob;

public class BobMenuIngredient
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public bool IsRequired { get; set; } = true;
    public bool IsBasic { get; set; }
    public List<string> SubstituteNames { get; set; } = [];

    public bool HasQuantity => Quantity > 0;
}
