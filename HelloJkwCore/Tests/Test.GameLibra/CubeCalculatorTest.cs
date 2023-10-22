using GameLibra.Service;

namespace Test.GameLibra;

public class CubeCalculatorTest
{
    [Theory]
    [InlineData("a==b", true)]
    [InlineData("a!=b", true)]
    [InlineData("a == b", true)]
    [InlineData("a != b", true)]
    [InlineData("abc==b+d", true)]
    [InlineData("a==b < c", false)]
    [InlineData("a==2b", false)]
    public void Test_input_formula(string formula, bool expected)
    {
        var cubeNames = new List<string> { "a", "b", "c", "d", "e" };

        var (result, _) = new CubeCalculator().TestValidFormula(formula, cubeNames);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("a", 1)]
    [InlineData("a+b", 3)]
    [InlineData("a+b+c", 6)]
    [InlineData("abcd", 10)]
    [InlineData("ab+cd", 10)]
    [InlineData("a+b-c-d", -4)]
    [InlineData("a+6", 7)]
    public void Test_expression(string expression, int expected)
    {
        var set = new Dictionary<string, int>
        {
            ["a"] = 1,
            ["b"] = 2,
            ["c"] = 3,
            ["d"] = 4,
            ["e"] = 5,
        };

        var result = new CubeCalculator().CalculateExpression(expression, set);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("a==b", false)]
    [InlineData("a!=b", true)]
    [InlineData("c==a+b", true)]
    [InlineData("c = a+b", true)]
    [InlineData("d > a+b", true)]
    [InlineData("de > a+b+c", true)]
    [InlineData("de = a+bc+c", true)]
    public void Test_formula(string formula, object expected)
    {
        var set = new Dictionary<string, int>
        {
            ["a"] = 1,
            ["b"] = 2,
            ["c"] = 3,
            ["d"] = 4,
            ["e"] = 5,
        };

        var result = new CubeCalculator().CalculateFormula(formula, set);

        Assert.Equal(expected, result);
    }
}