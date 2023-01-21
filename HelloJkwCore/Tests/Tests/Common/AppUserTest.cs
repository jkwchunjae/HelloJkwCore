namespace Tests.Common;

public class AppUserTest
{
    [Fact]
    public void AppUser객체가_달라도_ID가_같으면_같다()
    {
        var user1 = new AppUser("google", "111");
        var user2 = new AppUser("google", "111");

        Assert.True(user1 == user2);
    }
}
