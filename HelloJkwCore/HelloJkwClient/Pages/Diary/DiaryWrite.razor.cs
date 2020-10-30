using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloJkwClient.Pages.Diary
{
    public partial class DiaryWrite : ComponentBase
    {
        [Parameter]
        public string DiaryName { get; set; }

    }
}
