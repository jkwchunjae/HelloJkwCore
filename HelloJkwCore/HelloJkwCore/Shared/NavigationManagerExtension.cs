using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloJkwCore
{
    public static class NavigationManagerExtension
    {
        public static void GoLoginPage(this NavigationManager navi)
        {
            navi.NavigateTo("/login");
        }
    }
}
