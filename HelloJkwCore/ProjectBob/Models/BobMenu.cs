namespace ProjectBob;

public class BobMenu
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public BobMenuType Type { get; set; } = BobMenuType.SideDish;
    public List<BobMenuIngredient> Ingredients { get; set; } = [];
    public BobTasteLevel TasteLevel { get; set; } = BobTasteLevel.Unknown;
    public BobCookingDifficulty Difficulty { get; set; } = BobCookingDifficulty.Normal;
    public int CookingMinutes { get; set; } = 20;
    public bool IsGoodForBreakfast { get; set; }
    public bool CanMakeAhead { get; set; }
    public string Memo { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public bool HasName => !string.IsNullOrWhiteSpace(Name);
}
