using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSuFc
{
    public class MergeSplitOption
    {
        public List<MemberName> Names { get; set; }
        public double Probability { get; set; }
    }
    public class TeamSettingOption
    {
        public List<MergeSplitOption> SplitOptions { get; set; } = new();
        public List<MergeSplitOption> MergeOptions { get; set; } = new();
    }
}
