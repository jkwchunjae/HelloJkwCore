# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

HelloJkwCore is a modular ASP.NET Core Blazor Server application built on .NET 9.0. It's a multi-tenant web platform hosting several educational games and personal productivity tools, with a strong focus on Korean language features.

## Build & Development Commands

**Working Directory**: All commands should be run from the `HelloJkwCore/` subdirectory (not the repository root).

```bash
cd HelloJkwCore

# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Build for production
dotnet build --configuration Release

# Run the application
dotnet run --project HelloJkwCore

# Run tests
dotnet test

# Run specific test project
dotnet test Tests/Tests/Tests.csproj
dotnet test Tests/Test.GameLibra/Test.GameLibra.csproj

# Run tests with coverage
dotnet test /p:CollectCoverage=true
```

## Architecture

### Modular Blazor Server Application

The application uses a **pluggable module system** where feature modules are implemented as Razor Class Libraries that are dynamically loaded at runtime.

**Main Projects**:
- **HelloJkwCore**: Web host application, authentication, routing, and shared UI
- **Common**: Shared infrastructure library used by all modules
- **ProjectDiary**: Personal diary/journal with search, encryption, and image support
- **ProjectBaduk**: Go (Baduk) game tracking and diary
- **GameLibra**: Educational "Balance Scale" puzzle game (양팔저울)
- **ProjectPingpong**: Ping-pong tournament and match tracking
- **ProjectWorldCup**: FIFA World Cup prediction and betting simulation

### Dynamic Module Loading

Modules are registered in `Program.cs` using extension methods:
```csharp
builder.Services.AddDiaryService(builder.Configuration);
builder.Services.AddBadukService(builder.Configuration);
builder.Services.AddGameLibra(builder.Configuration);
```

And their assemblies are loaded in the routing configuration:
```csharp
app.MapRazorComponents<App>()
    .AddAdditionalAssemblies([
        typeof(Common.JkwPageBase).Assembly,
        typeof(ProjectDiary.DiaryService).Assembly,
        typeof(ProjectBaduk.BadukService).Assembly,
        typeof(GameLibra.LibraService).Assembly,
    ])
```

### File System Abstraction Layer

The codebase uses a custom `IFileSystemService` abstraction that supports multiple storage backends:
- **Local**: Local file system storage
- **Azure**: Azure Blob Storage (with container::path notation)
- **Dropbox**: Dropbox API
- **InMemory**: In-memory storage (for testing)
- **Backup**: Automatic failover wrapper (e.g., Azure with Dropbox backup)

Configuration in `appsettings.json`:
```json
"FileSystem": {
  "MainFileSystem": {
    "MainFileSystem": "Azure",
    "BackupFileSystem": "Dropbox",
    "UseBackup": true
  }
}
```

Each service can be configured to use a specific file system via keyed services. All data persistence uses JSON files stored in the configured file system - **there is no SQL database**.

### Base Page Pattern

All Blazor pages inherit from `Common.JkwPageBase`, which provides:
- Automatic authentication state handling via `OnAuthenticationReady()`
- User context (`AppUser User`)
- Navigation helpers
- Sealed lifecycle methods with virtual hooks for page-specific logic

When creating new pages, always inherit from `JkwPageBase` and override `OnAuthenticationReady()` instead of `OnInitializedAsync()`.

### Background Task Queue

For long-running operations (like search indexing), use `IBackgroundTaskQueue`:
```csharp
backgroundTaskQueue.QueueBackgroundWorkItem(async token => {
    // Your async work here
});
```

This prevents blocking HTTP requests and is configured in `Program.cs` with `QueuedHostedService`.

## Key Technologies

- **Framework**: ASP.NET Core 9.0, Blazor Server (Interactive Server render mode)
- **UI**: MudBlazor 8.13.0, TableViewer.MudBlazor 0.4.1
- **Authentication**: Google OAuth, KakaoTalk OAuth, ASP.NET Core Identity
- **Storage**: Azure Blob Storage, Dropbox API, local file system
- **Image Processing**: SixLabors.ImageSharp 3.1.11
- **Testing**: xUnit 2.9.3, Moq 4.20.72, FluentAssertions 8.7.1
- **Utilities**: Jkw.Extensions 1.0.13, System.Runtime.Caching

## Configuration

The application uses a layered configuration system:

1. `appsettings.json` - Base configuration (checked into source control with empty secrets)
2. `appsettings.user.json` - User-specific overrides (gitignored, contains actual credentials)

**Important**: Always use `appsettings.user.json` for local development credentials. Never commit real credentials to `appsettings.json`.

Required configuration sections:
- `HelloJkw.AuthOptions`: OAuth provider credentials (Google, KakaoTalk)
- `FileSystem`: Storage backend credentials (Azure connection strings, Dropbox tokens)
- Service-specific options: `DiaryService`, `BadukService`, `PingpongService`

## Korean Language Features

The application is designed for Korean users and includes:
- Default culture set to `ko-KR` in `Program.cs`
- KakaoTalk OAuth authentication
- Korean-specific game mechanics (세벌식 keyboard layout, 양팔저울 game)
- All user-facing text should be in Korean

## Testing Patterns

Tests use:
- **Moq**: For mocking dependencies (especially `IFileSystem`)
- **FluentAssertions**: For readable assertions
- **InMemoryFileSystem**: For isolated file system testing

Example test pattern:
```csharp
var mockFileSystem = new Mock<IFileSystem>();
// Setup mocks
var service = new MyService(mockFileSystem.Object);
// Assert with FluentAssertions
result.Should().Be(expected);
```

## Adding New Modules

To add a new module:

1. Create a new Razor Class Library project targeting `net10.0`
2. Add project reference to `Common`
3. Create a service class and extension method for dependency injection (e.g., `AddMyService()`)
4. Configure service in `appsettings.json` with file system paths
5. Register service in `Program.cs`
6. Add assembly to `MapRazorComponents()` in `Program.cs`
7. Create pages inheriting from `JkwPageBase`

## Security Notes

- Diary entries support encryption via password protection using `Common.Encryptor`
- Authentication uses ASP.NET Core Identity with OAuth providers
- File storage uses SAS URLs for Azure Blob Storage access
- User data is stored in configured cloud storage (Azure/Dropbox)
- **Never commit credentials**: Use `appsettings.user.json` for local secrets

## Recent Development Focus

Based on recent git history:
- Korean 3-key typing system (세벌식) implementation
- Balance scale game (양팔저울) UI improvements and bug fixes
- Image handling improvements with SAS URLs for diary pictures
- Tetration fractal visualization features
