using System.Net;
using System.Net.Http;
using ProjectKidsnote.Models.Account;
using ProjectKidsnote.Pages;

namespace Tests.Kidsnote;

public sealed class KidsnoteRegistrationTest
{
    [Fact]
    public async Task StoredConnectionRecoversAfterTemporaryFailure()
    {
        var attempts = 0;
        var profile = new UserInfo
        {
            User = new User
            {
                Username = "test", Type = "parent", Name = "Test Parent",
                CountryCode = "", Phone = "", Email = "", Description = ""
            }
        };

        var result = await KidsnoteRegistration.RestoreConnectionAsync(() =>
            ++attempts == 1
                ? Task.FromException<UserInfo>(new HttpRequestException(null, null, HttpStatusCode.ServiceUnavailable))
                : Task.FromResult(profile));

        result.Should().BeSameAs(profile);
        attempts.Should().Be(2);
    }

    [Theory]
    [InlineData(HttpStatusCode.Unauthorized, 1)]
    [InlineData(HttpStatusCode.Forbidden, 1)]
    [InlineData(HttpStatusCode.TooManyRequests, 1)]
    [InlineData(HttpStatusCode.ServiceUnavailable, 2)]
    public async Task StoredConnectionLimitsRetries(HttpStatusCode status, int expectedAttempts)
    {
        var attempts = 0;

        var error = await Assert.ThrowsAsync<HttpRequestException>(() =>
            KidsnoteRegistration.RestoreConnectionAsync(() =>
            {
                attempts++;
                return Task.FromException<UserInfo>(new HttpRequestException(null, null, status));
            }));

        error.StatusCode.Should().Be(status);
        attempts.Should().Be(expectedAttempts);
    }
}
