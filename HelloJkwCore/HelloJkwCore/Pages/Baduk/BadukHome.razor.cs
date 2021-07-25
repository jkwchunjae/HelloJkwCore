using Common;
using HelloJkwCore.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ProjectBaduk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloJkwCore.Pages.Baduk
{
    public partial class BadukHome : JkwPageBase
    {
        private BadukBoard Board { get; set; } = new BadukBoard(19);

        protected override Task OnPageInitializedAsync()
        {
            return Task.CompletedTask;
        }

        private void ClickCell(int row, int column)
        {
            Board.ClickCell(row, column);
            StateHasChanged();
        }
    }
}
