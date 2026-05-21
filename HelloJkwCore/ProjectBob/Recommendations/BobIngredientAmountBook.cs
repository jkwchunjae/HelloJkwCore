namespace ProjectBob;

public class BobIngredientAmountBook
{
    private readonly Dictionary<BobIngredientKey, IngredientAmount> _amounts = [];

    public IReadOnlyCollection<IngredientAmount> Items => _amounts.Values;

    public void Add(string name, decimal quantity, string unit)
    {
        if (string.IsNullOrWhiteSpace(name) || quantity <= 0)
            return;

        var key = BobIngredientKey.From(name, unit);
        if (_amounts.TryGetValue(key, out var amount))
        {
            amount.Quantity += quantity;
        }
        else
        {
            _amounts[key] = new IngredientAmount(name.Trim(), quantity, unit.Trim());
        }
    }

    public decimal GetQuantity(string name, string unit)
    {
        return _amounts.TryGetValue(BobIngredientKey.From(name, unit), out var amount)
            ? amount.Quantity
            : 0;
    }

    public void Remove(string name, decimal quantity, string unit)
    {
        var key = BobIngredientKey.From(name, unit);
        if (!_amounts.TryGetValue(key, out var amount))
            return;

        amount.Quantity = Math.Max(0, amount.Quantity - quantity);
    }

    public BobIngredientAmountBook Clone()
    {
        var book = new BobIngredientAmountBook();
        foreach (var amount in _amounts.Values)
        {
            book.Add(amount.Name, amount.Quantity, amount.Unit);
        }
        return book;
    }

    public static BobIngredientAmountBook FromFridge(IEnumerable<BobFridgeItem> fridgeItems)
    {
        var book = new BobIngredientAmountBook();
        foreach (var item in fridgeItems)
        {
            book.Add(item.Name, item.Quantity, item.Unit);
        }
        return book;
    }

    public static BobIngredientAmountBook FromMenus(IEnumerable<BobMenu> menus)
    {
        var book = new BobIngredientAmountBook();
        foreach (var ingredient in menus.SelectMany(menu => menu.Ingredients))
        {
            // 기본 재료는 장보기와 부족 재료 계산에서 제외한다.
            if (!ingredient.IsBasic)
            {
                book.Add(ingredient.Name, ingredient.Quantity, ingredient.Unit);
            }
        }
        return book;
    }

    public class IngredientAmount
    {
        public IngredientAmount(string name, decimal quantity, string unit)
        {
            Name = name;
            Quantity = quantity;
            Unit = unit;
        }

        public string Name { get; }
        public decimal Quantity { get; set; }
        public string Unit { get; }
    }
}
