using System.Text.Json.Serialization;
using HelloJkwCore;
using HelloJkwCore.Authentication;
using HelloJkwCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.user.json", optional: true);

var coreOption = builder.Configuration.GetSection("HelloJkw").Get<CoreOption>()!;
builder.Services.AddSingleton(coreOption);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHostedService<QueuedHostedService>();
builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
builder.Services.AddHttpClient();

builder.Services.AddMudServices();

#region FileSystem

var fsOption = builder.Configuration.GetSection("FileSystem").Get<FileSystemOption>()!;
builder.Services.AddSingleton(fsOption);
builder.Services.AddSingleton<IFileSystemService, FileSystemService>();
builder.Services.AddSingleton<ISerializer, Json>(serviceProvider =>
{
    var converters = serviceProvider.GetServices<JsonConverter>();
    return new Json(converters);
});
builder.Services.AddInMemoryFileSystem();
builder.Services.AddLocalFileSystem();
if (fsOption.Dropbox != null)
{
    builder.Services.AddDropboxFileSystem(fsOption.Dropbox);
}
if (fsOption.Azure != null)
{
    builder.Services.AddAzureFileSystem(fsOption.Azure);
}

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

builder.Services.AddSingleton<IUserStore<AppUser>, AppUserStore>(provider =>
{
    var option = provider.GetRequiredService<CoreOption>();
    var fileSystemService = provider.GetRequiredService<IFileSystemService>();
    var fileSystem = fileSystemService.GetFileSystem(option.UserStoreFileSystem, option.Path);
    return new AppUserStore(fileSystem, provider.GetRequiredService<ILoggerFactory>());
});
builder.Services.AddSingleton<IRoleStore<ApplicationRole>, AppRoleStore>();
builder.Services.AddSingleton<JsonConverter>(new StringIdTextJsonConverter<UserId>(id => new UserId(id)));

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
