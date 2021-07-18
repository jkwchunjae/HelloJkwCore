using HelloJkwCore.Shared;
using ProjectBaduk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloJkwCore.Pages.Baduk
{
    public partial class BadukHome : JkwPageBase
    {
        private int Size => Board?.Size ?? 19;

        private Dictionary<int, List<int>> PointArray = new()
        {
            [19] = new() { 4, 10, 16 },
            [15] = new() { 4, 8, 12 },
            [13] = new() { 4, 7, 10 },
            [9] = new() { 5 },
        };
        private BadukBoard Board { get; set; } = new BadukBoard(19);
        private string IndexText => $"{Board.CurrentIndex} / {Board.LastIndex}";

        protected override Task OnPageInitializedAsync()
        {
            return Task.CompletedTask;
        }
    }
}
