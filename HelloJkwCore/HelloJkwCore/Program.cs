using HelloJkwCore;
using HelloJkwCore.Authentication;
using HelloJkwCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.user.json", optional: true);

var coreOption = CoreOption.Create(builder.Configuration);
builder.Services.AddSingleton(coreOption);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHostedService<QueuedHostedService>();
builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
builder.Services.AddHttpClient();

builder.Services.AddMudServices();

#region FileSystem

var fsOption = new FileSystemOption();
builder.Configuration.GetSection("FileSystem").Bind(fsOption);
builder.Services.AddSingleton(fsOption);
builder.Services.AddSingleton<IFileSystemService, FileSystemService>();

#endregion

#region Authentication

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

AuthUtil authUtil = new AuthUtil(coreOption);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddGoogleAuthentication(authUtil.GetAuthOption(AuthProvider.Google))
    .AddKakaoAuthentication(authUtil.GetAuthOption(AuthProvider.KakaoTalk))
    .AddIdentityCookies();

builder.Services
    .AddIdentityCore<AppUser>()
    .AddUserManager<AppUserManager>()
    .AddRoles<ApplicationRole>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IUserStore<AppUser>, AppUserStore>();
builder.Services.AddSingleton<IRoleStore<ApplicationRole>, AppRoleStore>();

#endregion

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Diary",
        policy => policy.RequireAuthenticatedUser());
});
builder.Services.AddDiaryService(builder.Configuration);
builder.Services.AddBadukService(builder.Configuration);
builder.Services.AddGameLibra(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
