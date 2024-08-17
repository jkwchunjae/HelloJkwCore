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
builder.Services.AddRazorComponents();

builder.Services.AddMudServices();

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
    .AddIdentityCore<AppUser>()
    .AddUserManager<AppUserManager>()
    .AddRoles<ApplicationRole>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IUserStore<AppUser>, AppUserStore>();
builder.Services.AddSingleton<IRoleStore<ApplicationRole>, AppRoleStore>();

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

app.MapRazorComponents<App>();

app.Run();
