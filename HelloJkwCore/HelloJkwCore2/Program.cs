using HelloJkwCore2.Authentication;
using HelloJkwCore2.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using MudBlazor.Services;
using HelloJkwCore2;

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
    .AddGoogleAuthentication(authUtil)
    .AddKakaoAuthentication(authUtil)
    .AddIdentityCookies();

builder.Services
    .AddIdentityCore<ApplicationUser>()
    .AddUserManager<AppUserManager>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IUserStore<ApplicationUser>, AppUserStore>();
// builder.Services.AddSingleton<IRoleStore<ApplicationRole>, AppRoleStore>();

#endregion

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
