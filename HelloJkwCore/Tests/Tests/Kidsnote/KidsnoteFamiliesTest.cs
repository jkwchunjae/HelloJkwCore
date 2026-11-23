using Microsoft.Extensions.Configuration;
using ProjectKidsnote.Models.Authentication;
using ProjectKidsnote.Models.Parents;
using ProjectKidsnote.Services;

namespace Tests.Kidsnote;

public sealed class KidsnoteFamiliesTest
{
    private readonly AppUser owner = new() { Id = new UserId("parent"), Roles = [UserRole.Kidsnote] };
    private readonly AppUser partner = new() { Id = new UserId("partner") };
    private readonly UserId viewer = new("viewer");
    private readonly InMemoryFileSystem files;
    private readonly KidsnoteFamilies families;

    public KidsnoteFamiliesTest()
    {
        var paths = new Paths(new PathMap { InMemory = new() { ["ParentPath"] = "/parents" } }, FileSystemType.InMemory);
        files = new(paths, new Json([new StringIdTextJsonConverter<UserId>(id => new UserId(id))]));
        families = new(files);
    }

    private KidsParent Family(AppUser user, string name) => new()
    {
        User = user.Id, Auth = new KidsnoteAuth { EncryptedAuthInfo = "encrypted" },
        Partners = [partner.Id], Kids = [new Kid { ChildId = 1, NicknameForUrl = name, AllowedList = [viewer] }]
    };

    [Fact]
    public async Task SavesByOwnerAndRechecksPartnerAndReaderRights()
    {
        var draft = Family(owner, "hong");
        draft.Kids.Add(new Kid { ChildId = 2, NicknameForUrl = "soo" });
        await families.SaveAsync(owner, draft);
        (await files.GetFilesAsync(p => p["ParentPath"])).Should().Equal("kidsnote.parent.json");

        var loaded = (await families.ReadAsync()).Single();
        loaded.CanRead(owner.Id, loaded.Kids[1]).Should().BeTrue();
        loaded.CanRead(partner.Id, loaded.Kids[1]).Should().BeTrue();
        loaded.CanRead(viewer, loaded.Kids[0]).Should().BeTrue();
        loaded.CanRead(viewer, loaded.Kids[1]).Should().BeFalse();
        loaded.AllowedList.Add(viewer);
        loaded.CanRead(viewer, loaded.Kids[1]).Should().BeTrue();
        loaded.CanManage(viewer).Should().BeFalse();

        loaded.Partners.Clear();
        loaded.Kids.RemoveAt(1);
        await families.SaveAsync(partner, loaded);
        var updated = (await families.ReadAsync()).Single();
        updated.Kids.Should().ContainSingle();
        updated.CanRead(partner.Id, updated.Kids[0]).Should().BeFalse();
        await Assert.ThrowsAsync<InvalidOperationException>(() => families.SaveAsync(partner, loaded));
    }

    [Fact]
    public async Task RejectsConflictingNicknamesAndStaleEdits()
    {
        var first = Family(owner, "hong");
        await families.SaveAsync(owner, first);
        var other = new AppUser { Id = new UserId("other"), Roles = [UserRole.Kidsnote] };
        await Assert.ThrowsAsync<InvalidOperationException>(() => families.SaveAsync(other, Family(other, "hong")));
        var stale = (await families.ReadAsync()).Single();
        first.Kids.Add(new Kid { ChildId = 2, NicknameForUrl = "hong" });
        await Assert.ThrowsAsync<InvalidOperationException>(() => families.SaveAsync(owner, first));
        first.Kids[1].NicknameForUrl = "soo";
        await families.SaveAsync(owner, first);
        await Assert.ThrowsAsync<InvalidOperationException>(() => families.SaveAsync(partner, stale));
        (await families.ReadAsync()).Single().Kids.Should().HaveCount(2);
    }

    [Fact]
    public async Task SavesConnectionBeforeChildrenAndKeepsPreviousStateOnInvalidEdit()
    {
        var connection = await families.UpdateAsync(owner, new KidsParent { User = owner.Id },
            p => p.Auth = new KidsnoteAuth { EncryptedAuthInfo = "encrypted" });
        var saved = (await families.ReadAsync()).Single();
        saved.Kids.Should().BeEmpty();
        saved.Auth.EncryptedAuthInfo.Should().Be("encrypted");

        var registered = await families.UpdateAsync(owner, connection,
            p => p.Kids.Add(new Kid { ChildId = 1, NicknameForUrl = "hong" }));
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            families.UpdateAsync(owner, registered, p => p.Kids[0].NicknameForUrl = "invalid-name"));
        registered.Kids.Single().NicknameForUrl.Should().Be("hong");
        (await families.ReadAsync()).Single().Kids.Single().NicknameForUrl.Should().Be("hong");
    }

    [Fact]
    public void CredentialsRoundTripOnlyForTheirOwner()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            { ["Kidsnote:AuthEncrypt:Secret"] = "test-only" }).Build();
        var credentials = new KidsnoteCredentials(config);
        var login = new KidsnoteLoginRequest("kidsnote-id", "test-password");
        var encrypted = credentials.Protect(owner.Id, login);
        encrypted.Should().NotContain(login.Password);
        credentials.Open(owner.Id, encrypted).Should().Be(login);
        Assert.ThrowsAny<System.Security.Cryptography.CryptographicException>(() => credentials.Open(partner.Id, encrypted));
    }
}
