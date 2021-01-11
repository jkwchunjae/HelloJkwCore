using Common;
using Common.Dropbox;
using HelloJkwCore.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectDiary;
using System.Net.Http;

namespace HelloJkwCore
{
    public class Startup
    {
        readonly CoreOption _coreOption;

        private AuthUtil CreateAuthUtil(IConfiguration configuration)
        {
            var pathOption = new PathOption();
            configuration.GetSection("Path").Bind(pathOption);

            var fsOption = new FileSystemOption();
            configuration.GetSection("FileSystem").Bind(fsOption);

            var fsService = new FileSystemService(fsOption, pathOption, null, null);
            var fs = fsService.GetFileSystem(_coreOption.AuthFileSystem);

            return new AuthUtil(fs);
        }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            _coreOption = CoreOption.Create(Configuration);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_coreOption);

            #region Authentication

            var authUtil = CreateAuthUtil(Configuration);

            var googleAuthOption = authUtil.GetAuthOption(AuthProvider.Google);
            var kakaoAuthOption = authUtil.GetAuthOption(AuthProvider.KakaoTalk);

            services.AddIdentityCore<AppUser>()
                .AddUserManager<AppUserManager<AppUser>>()
                .AddSignInManager<SignInManager<AppUser>>()
                .AddRoles<IdentityRole>()
                .AddRoleStore<RoleStore>();

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

            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                   .RequireAuthenticatedUser()
                   .Build();
            });

            services.AddHelloJkwPolicy();

            #endregion

            #region ASP.NET

            services.AddHttpContextAccessor();
            services.AddScoped<HttpContextAccessor>();
            services.AddHttpClient();
            services.AddScoped<HttpClient>();

            services.AddSingleton(Configuration);

            services.AddRazorPages();
            services.AddServerSideBlazor();

            //services.AddHostedService<TimedHostedService>();
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

            #endregion

            #region FileSystem

            var pathOption = new PathOption();
            Configuration.GetSection("Path").Bind(pathOption);
            services.AddSingleton(pathOption);

            var fsOption = new FileSystemOption();
            Configuration.GetSection("FileSystem").Bind(fsOption);
            services.AddSingleton(fsOption);

            services.AddSingleton<IFileSystemService, FileSystemService>();

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
