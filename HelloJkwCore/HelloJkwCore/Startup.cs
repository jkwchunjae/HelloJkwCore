using Common;
using Common.FileSystem;
using Dropbox.Api;
using HelloJkwCore.Authentication;
using HelloJkwCore.Data;
using HelloJkwCore.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HelloJkwCore
{
    public class Startup
    {
        readonly CoreOption _coreOption;

        readonly AuthUtil _authUtil;

        readonly IFileSystem _dropboxFileSystem;
        readonly IFileSystem _localFileSystem;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            PathOf.SetConfiguration(Configuration);
            _coreOption = CoreOption.Create(Configuration);

            var dropboxClient = GetDropboxClient(_coreOption);

            _localFileSystem = new LocalFileSystem(new UTF8Encoding(false));
            _dropboxFileSystem = new DropboxFileSystem(dropboxClient);

            if (_coreOption.UseLocalDropbox)
            {
                _authUtil = new AuthUtil(_localFileSystem);
            }
            else
            {
                _authUtil = new AuthUtil(_dropboxFileSystem);
            }
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var googleAuthOption = _authUtil.GetAuthOption(AuthProvider.Google);
            var kakaoAuthOption = _authUtil.GetAuthOption(AuthProvider.KakaoTalk);

            services.AddIdentityCore<AppUser>()
                .AddUserManager<AppUserManager<AppUser>>()
                .AddSignInManager<SignInManager<AppUser>>();

            services.AddSingleton<IUserStore<AppUser>, UserStore>();

            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();

            services
                //.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                //.AddCookie()
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                    options.DefaultSignOutScheme = IdentityConstants.ApplicationScheme;
                })
                .AddCookie(IdentityConstants.ExternalScheme)
                .AddCookie(IdentityConstants.ApplicationScheme)
                .AddGoogle(options =>
                {
                    options.ClientId = googleAuthOption?.ClientId;
                    options.ClientSecret = googleAuthOption?.ClientSecret;
                    options.CallbackPath = googleAuthOption?.Callback;
                    options.ClaimActions.MapJsonKey("urn:google:profile", "link");
                    options.ClaimActions.MapJsonKey("urn:google:image", "picture");
                })
                .AddKakaoTalk(options =>
                {
                    options.ClientId = kakaoAuthOption?.ClientId;
                    options.ClientSecret = kakaoAuthOption?.ClientSecret;
                    options.CallbackPath = kakaoAuthOption?.Callback;
                });

            if (_coreOption.UseLocalDropbox)
            {
                services.AddSingleton(_localFileSystem);
            }
            else
            {
                services.AddSingleton(_dropboxFileSystem);
            }

            services.AddHttpContextAccessor();
            services.AddScoped<HttpContextAccessor>();
            //// Required for HttpClient support in the Blazor Client project
            services.AddHttpClient();
            services.AddScoped<HttpClient>();
            // Pass settings to other components
            services.AddSingleton(Configuration);

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton<WeatherForecastService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }

        private DropboxClient GetDropboxClient(CoreOption option)
        {
            if (option.UseLocalDropbox)
            {
                var localFileSystem = new LocalFileSystem(new UTF8Encoding(false));
                var oauthPath = Configuration.GetPath(PathType.OAuthOption);
                var task = localFileSystem.ReadJsonAsync<List<OAuthOption>>(oauthPath);
                task.Wait();
                var dropboxOption = task.Result.FirstOrDefault(x => x.Provider == AuthProvider.Dropbox);

                return new DropboxClient(dropboxOption.RefreshToken, dropboxOption.ClientId, dropboxOption.ClientSecret);
            }
            else
            {
                var dropboxRefreshToken = Configuration["Auth:Dropbox:RefreshToken"];
                var dropboxClientId = Configuration["Auth:Dropbox:ClientId"];
                var dropboxClientSecret = Configuration["Auth:Dropbox:ClientSecret"];

                return new DropboxClient(dropboxRefreshToken, dropboxClientId, dropboxClientSecret);
            }
        }
    }
}
