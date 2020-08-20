using HelloJkwService.Reporra;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloJkwClient.Pages.Reporra
{
    public partial class ReporraBoardComponent : ComponentBase
    {
        private string cssReporraBoard;

        [Parameter]
        public ReporraGame Game { get; set; }
        [Parameter]
        public IReporraUser User { get; set; }

        ReporraBoard Board => Game?.Board;
        bool Reverse => Game?.Users.First() == User;

        IEnumerable<int> Sequence =>
            Enumerable.Range(0, Board.Size)
                .Select(x => Reverse ? Board.Size - x - 1 : x);
    }
}
