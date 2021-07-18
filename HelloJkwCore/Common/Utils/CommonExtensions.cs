using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class CommonExtensions
    {
        public static bool Between(this int value, int min, int max)
        {
            return value >= min && value < max;
        }
    }
}
