using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HelloJkwCore.Shared
{
    public partial class NavMenu
    {
        [Inject]
        IHttpContextAccessor _httpContextAccessor { get; set; }
        //@inject HttpClient Http
        [Inject]
        AuthenticationStateProvider _authenticationStateProvider { get; set; }

        private bool collapseNavMenu = true;
        private ClaimsPrincipal User;
        private string GivenName = string.Empty;
        private string Surname = string.Empty;
        private string Avatar;

        private bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

        private string NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }

        protected override async Task OnInitializedAsync()
        {
            base.OnInitialized();
            try
            {
                var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
                User = authState.User;

                // Set the user to determine if they are logged in
                //User = _httpContextAccessor.HttpContext.User;
                // Try to get the GivenName

                if (!IsAuthenticated)
                    return;

                var givenName =
                    _httpContextAccessor.HttpContext.User
                    .FindFirst(ClaimTypes.GivenName);
                if (givenName != null)
                {
                    GivenName = givenName.Value;
                }
                else
                {
                    GivenName = User?.Identity?.Name;
                }
                // Try to get the Surname
                var surname =
                    _httpContextAccessor.HttpContext.User
                    .FindFirst(ClaimTypes.Surname);
                if (surname != null)
                {
                    Surname = surname.Value;
                }
                else
                {
                    Surname = "";
                }
                // Try to get Avatar
                var avatar =
                _httpContextAccessor.HttpContext.User
                .FindFirst("urn:google:image");
                if (avatar != null)
                {
                    Avatar = avatar.Value;
                }
                else
                {
                    Avatar = "";
                }
            }
            catch { }
        }

    }
}
