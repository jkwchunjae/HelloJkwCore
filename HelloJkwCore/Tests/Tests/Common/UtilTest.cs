namespace Tests.Common;

public class UtilTest
{
    [Theory]
    [InlineData(57880, -3, 7, 58000)]
    [InlineData(16206, -3, 7, 16000)]
    [InlineData(56118, -3, 7, 56000)]
    [InlineData(17245, -3, 7, 17000)]
    [InlineData(26196, -3, 7, 26000)]
    [InlineData(57578, -3, 7, 57000)]
    [InlineData(32987, -3, 7, 33000)]
    [InlineData(15717, -3, 7, 16000)]
    [InlineData(86403, -3, 7, 86000)]
    [InlineData(17867, -3, 7, 18000)]
    [InlineData(49100, -3, 7, 49000)]
    [InlineData(93488, -3, 7, 93000)]
    [InlineData(22760, -3, 7, 23000)]
    [InlineData(64111, -3, 7, 64000)]
    [InlineData(39127, -3, 7, 39000)]
    [InlineData(52525, -3, 7, 52000)]
    [InlineData(24895, -3, 7, 25000)]
    [InlineData(82815, -3, 7, 83000)]
    [InlineData(85255, -3, 7, 85000)]
    [InlineData(73174, -3, 7, 73000)]
    [InlineData(22357, -3, 7, 22000)]
    [InlineData(27022, -3, 7, 27000)]
    [InlineData(76835, -3, 7, 77000)]
    [InlineData(48490, -3, 7, 48000)]
    [InlineData(85043, -3, 7, 85000)]
    [InlineData(84663, -3, 7, 84000)]
    [InlineData(76575, -3, 7, 76000)]
    [InlineData(64719, -3, 7, 65000)]
    [InlineData(37225, -3, 7, 37000)]
    [InlineData(76301, -3, 7, 76000)]
    [InlineData(38100, -3, 7, 38000)]
    public void RoundNTest_for_worldcup_betting_7(int value, int digits, int N, int expected)
    {
        var result = value.RoundN(digits, N);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(57880, -3, 8, 58000)]
    [InlineData(16206, -3, 8, 16000)]
    [InlineData(56118, -3, 8, 56000)]
    [InlineData(17245, -3, 8, 17000)]
    [InlineData(26196, -3, 8, 26000)]
    [InlineData(57578, -3, 8, 57000)]
    [InlineData(32987, -3, 8, 33000)]
    [InlineData(15717, -3, 8, 15000)]
    [InlineData(86403, -3, 8, 86000)]
    [InlineData(17867, -3, 8, 18000)]
    [InlineData(49100, -3, 8, 49000)]
    [InlineData(93488, -3, 8, 93000)]
    [InlineData(22760, -3, 8, 22000)]
    [InlineData(64111, -3, 8, 64000)]
    [InlineData(39127, -3, 8, 39000)]
    [InlineData(52525, -3, 8, 52000)]
    [InlineData(24895, -3, 8, 25000)]
    [InlineData(82815, -3, 8, 83000)]
    [InlineData(85255, -3, 8, 85000)]
    [InlineData(73174, -3, 8, 73000)]
    [InlineData(22357, -3, 8, 22000)]
    [InlineData(27022, -3, 8, 27000)]
    [InlineData(76835, -3, 8, 77000)]
    [InlineData(48490, -3, 8, 48000)]
    [InlineData(85043, -3, 8, 85000)]
    [InlineData(84663, -3, 8, 84000)]
    [InlineData(76575, -3, 8, 76000)]
    [InlineData(64719, -3, 8, 64000)]
    [InlineData(37225, -3, 8, 37000)]
    [InlineData(76301, -3, 8, 76000)]
    [InlineData(38100, -3, 8, 38000)]
    public void RoundNTest_for_worldcup_betting_8(int value, int digits, int N, int expected)
    {
        var result = value.RoundN(digits, N);

        Assert.Equal(expected, result);
    }

}