namespace ProjectBob;

public class BobMenuAvailability
{
    public BobMenu Menu { get; set; } = new();
    public List<BobMissingIngredient> MissingIngredients { get; set; } = [];
    public bool HasRestrictedIngredient { get; set; }
    public bool HasDislikedIngredient { get; set; }
    public int ReusableIngredientScore { get; set; }
    public DateOnly? LastServedDate { get; set; }

    public bool CanCook => !HasRestrictedIngredient && MissingIngredients.Count == 0;
    public bool IsNearCandidate => !HasRestrictedIngredient && MissingIngredients.Count is >= 1 and <= 2;
}
