[assembly:System.Runtime.CompilerServices.InternalsVisibleTo("Test.GameLibra")]

namespace GameLibra.Service;

internal class CubeCalculator
{
    static readonly Dictionary<string, Func<int, int, bool>> compareDic = new()
    {
        ["=="] = (a, b) => a == b,
        ["!="] = (a, b) => a != b,
        ["<>"] = (a, b) => a != b,
        ["<="] = (a, b) => a <= b,
        [">="] = (a, b) => a >= b,
        ["="] = (a, b) => a == b,
        ["<"] = (a, b) => a < b,
        [">"] = (a, b) => a > b,
    };
    public bool TestValidFormula(string formula, List<string> cubeNames)
    {
        formula = formula.Replace(" ", "");

        if (formula.StartsWith('-'))
        {
            return false;
        }
        if (compareDic.Any(x => formula.Contains(x.Key)))
        {
            var first = compareDic.First(x => formula.Contains(x.Key));
            var operatorr = first.Key;
            var func = first.Value;
            var split = formula.Split(compareDic.Keys.ToArray(), StringSplitOptions.None);
            if (split.Length == 2)
            {
                var left = split[0].Trim();
                var right = split[1].Trim();
                if (!TestValidFormula(left, cubeNames))
                {
                    return false;
                }
                if (!TestValidFormula(right, cubeNames))
                {
                    return false;
                }
                return true;
            }
        }
        var split2 = formula.Split('+', '-').SelectMany(x => x).ToArray();
        if (split2.All(x => cubeNames.Contains(x.ToString())))
        {
            return true;
        }
        return false;
    }
    public object CalculateFormula(string formula, IReadOnlyDictionary<string, int> set)
    {
        formula = formula.Replace(" ", "");
        if (compareDic.Any(x => formula.Contains(x.Key)))
        {
            var first = compareDic.First(x => formula.Contains(x.Key));
            var operatorr = first.Key;
            var func = first.Value;
            var split = formula.Split(operatorr);
            if (split.Length != 2)
            {
                return null;
            }
            var left = split[0].Trim();
            var right = split[1].Trim();
            var leftValue = CalculateExpression(left, set);
            var rightValue = CalculateExpression(right, set);
            return func(leftValue, rightValue);
        }
        return CalculateExpression(formula, set);
    }
    public int CalculateExpression(string formula, IReadOnlyDictionary<string, int> set)
    {
        formula = formula.Replace(" ", "");
        if (formula.Contains("+"))
        {
            return formula.Split('+')
                .Sum(split => CalculateExpression(split, set));
        }
        else if (formula.Contains("-"))
        {
            var split = formula.Split('-');
            var first = split[0];
            var firstValue = CalculateExpression(first, set);
            var rest = split.Skip(1).Select(x => CalculateExpression(x, set));
            return firstValue - rest.Sum();
        }
        else
        {
            return formula
                .Sum(x => set[x.ToString()]);
        }
    }
}