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

        public ReporraBoard Board => Game?.Board;
    }
}
