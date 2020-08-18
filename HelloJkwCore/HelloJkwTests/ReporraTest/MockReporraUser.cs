using HelloJkwService.Reporra;
using JkwExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloJkwTests.ReporraTest
{
    public class MockReporraUser : ReporraUser
    {
        public override Task<ReporraColor> WaitUserChoiceAsync(int size, string color)
        {
            var remainColors = color.Where(x => x != color.First() && x != color.Last())
                .Distinct()
                .ToList();

            if (remainColors.Empty())
                throw new ArgumentException("이미 끝났음");

            var nextColor = (ReporraColor)(remainColors.GetRandom() - 'A');

            return Task.FromResult(nextColor);
        }
    }
}
