using System.Text.Json.Serialization;
using HelloJkwCore.Authentication;
using Microsoft.Extensions.Logging.Abstractions;

namespace Tests.Common;

public class AppUserStoreTest
{
    [Fact]
    public async Task 생성한_Admin_사용자는_전체_사용자_목록에_포함된다()
    {
        var store = CreateStore();
        var user = new AppUser
        {
            Id = new UserId("dev.admin"),
            UserName = "dev.admin",
            NickName = "dev.admin",
        };

        var createResult = await store.CreateAsync(user, CancellationToken.None);
        await store.AddToRoleAsync(user, nameof(UserRole.Admin), CancellationToken.None);

        var users = await store.GetUsersInRoleAsync("all", CancellationToken.None);

        createResult.Succeeded.Should().BeTrue();
        users.Should().ContainSingle(foundUser =>
            foundUser.Id == user.Id &&
            foundUser.NickName == user.Id.Id &&
            foundUser.HasRole(UserRole.Admin));
    }

    private static AppUserStore CreateStore()
    {
        var files = new Dictionary<string, string>();
        var pathMap = new PathMap
        {
            InMemory = new Dictionary<string, string>
            {
                ["Users"] = "/users",
                ["Logins"] = "/logins",
            },
        };
        JsonConverter[] converters = [
            new StringIdTextJsonConverter<UserId>(id => new UserId(id))
        ];
        var paths = new Paths(pathMap, FileSystemType.InMemory);
        var serializer = new Json(converters);
        var fileSystem = new Mock<IFileSystem>();

        fileSystem
            .Setup(fs => fs.WriteJsonAsync(
                It.IsAny<Func<Paths, string>>(),
                It.IsAny<AppUser>(),
                It.IsAny<CancellationToken>()))
            .Returns((Func<Paths, string> pathFunc, AppUser user, CancellationToken _) =>
            {
                files[pathFunc(paths)] = serializer.Serialize(user);
                return Task.FromResult(true);
            });

        fileSystem
            .Setup(fs => fs.GetFilesAsync(
                It.IsAny<Func<Paths, string>>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns((Func<Paths, string> _, string extension, CancellationToken _) =>
                Task.FromResult(files.Keys
                    .Select(Path.GetFileName)
                    .Where(fileName => extension is null || fileName.EndsWith(extension))
                    .ToList()));

        fileSystem
            .Setup(fs => fs.ReadJsonAsync<AppUser>(
                It.IsAny<Func<Paths, string>>(),
                It.IsAny<CancellationToken>()))
            .Returns((Func<Paths, string> pathFunc, CancellationToken _) =>
                Task.FromResult(serializer.Deserialize<AppUser>(files[pathFunc(paths)])));

        return new AppUserStore(fileSystem.Object, NullLoggerFactory.Instance);
    }
}
