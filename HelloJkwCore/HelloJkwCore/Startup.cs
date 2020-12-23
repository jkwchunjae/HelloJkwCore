using Common;
using Common.Authentication;
using Common.Dropbox;
using Common.Extensions;
using Common.FileSystem;
using Common.User;
using Dropbox.Api;
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
using ProjectDiary;
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

        readonly FileSystemService _fileSystemService;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            var pathConfig = new Dictionary<string, string>();
            configuration.GetSection("Path").Bind(pathConfig);
            ConfigurationPathExtensions.SetPathConfig(pathConfig
                .Where(x => Enum.TryParse<PathType>(x.Key, out var _))
                .ToDictionary(x => Enum.Parse<PathType>(x.Key), x => x.Value));

            _coreOption = CoreOption.Create(Configuration);

            var fsOption = new FileSystemOption();
            Configuration.GetSection("FileSystem").Bind(fsOption);
            _fileSystemService = new FileSystemService(fsOption);
            _authUtil = new AuthUtil(_fileSystemService.GetFileSystem(FileSystemType.Dropbox));
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            #region Authentication

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

            #endregion

            services.AddHttpContextAccessor();
            services.AddScoped<HttpContextAccessor>();
            services.AddHttpClient();
            services.AddScoped<HttpClient>();

            services.AddSingleton(Configuration);

            services.AddRazorPages();
            services.AddServerSideBlazor();

            #region FileSystem

            services.AddSingleton(_fileSystemService);

            #endregion

            services.AddDiaryService(Configuration);
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
    }
}
