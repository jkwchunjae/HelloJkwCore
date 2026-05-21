namespace ProjectBob;

public readonly record struct BobIngredientKey(string Name, string Unit)
{
    public static BobIngredientKey From(string name, string unit)
        => new(BobIngredientRules.NormalizeName(name), BobIngredientRules.NormalizeUnit(unit));
}
