namespace Tests.Common;

public class AppUserTest
{
    [Fact]
    public void AppUser객체가_달라도_ID가_같으면_같다()
    {
        var user1 = new AppUser() { Id = new UserId("google.111") };
        var user2 = new AppUser() { Id = new UserId("google.111") };

        user1.Should().Be(user2);
    }
}
