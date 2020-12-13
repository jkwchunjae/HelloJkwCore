using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Common.User;
using HelloJkwCore.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HelloJkwCore.Pages
{
    public class LoginExternalModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public LoginExternalModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult OnGetAsync(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Page("./LoginExternal", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        public async Task<IActionResult> OnGetCallbackAsync(
            string returnUrl = null, string remoteError = null)
        {
            // Get the information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true);
            if (result.Succeeded)
            {
                await HttpContext.SignInAsync(info.Principal);
            }
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var name = info.Principal.FindFirstValue(ClaimTypes.Name);
                var newUser = new AppUser(info.LoginProvider, info.ProviderKey)
                {
                    UserName = name,
                    Email = email,
                    CreateTime = DateTime.Now,
                };
                await _userManager.CreateAsync(newUser);
                await HttpContext.SignInAsync(info.Principal);
            }

            //var GoogleUser = this.User.Identities.FirstOrDefault();
            //if (GoogleUser.IsAuthenticated)
            //{
            //    var authProperties = new AuthenticationProperties
            //    {
            //        IsPersistent = true,
            //        RedirectUri = this.Request.Host.Value
            //    };
            //    await HttpContext.SignInAsync(
            //        CookieAuthenticationDefaults.AuthenticationScheme,
            //        new ClaimsPrincipal(GoogleUser),
            //        authProperties);
            //}

            return LocalRedirect("/");
        }

    }
}
