using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common;

public static class CommonExtensions
{
    public static bool Between(this int value, int min, int max)
    {
        return value >= min && value < max;
    }

    public static int RoundN(this int value, int digits, int N)
    {
        if (N <= 0 || N > 9)
            throw new ArgumentException("N은 1부터 9사이 값이어야합니다.");

        var result = value;
        var tens = (int)Math.Pow(10, -digits);
        result /= tens;

        var up = (value / (tens / 10)) % 10 >= N; // digits에 해당하는 숫자가 N 이상인지? N이상이면 올림한다.
        result += (up ? 1 : 0);
        result  *= tens;

        return result;
    }
}