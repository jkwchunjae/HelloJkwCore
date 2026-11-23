using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Common;
using Microsoft.Extensions.Configuration;
using ProjectKidsnote.Models.Authentication;

namespace ProjectKidsnote.Services;

internal sealed class KidsnoteCredentials(IConfiguration configuration)
{
    private AesGcm CreateCipher()
    {
        var secret = configuration["Kidsnote:AuthEncrypt:Secret"];
        if (string.IsNullOrWhiteSpace(secret))
            throw new InvalidOperationException("서버에 키즈노트 인증 정보 암호화 키가 설정되어 있지 않습니다.");
        return new AesGcm(SHA256.HashData(Encoding.UTF8.GetBytes(secret)), 16);
    }

    public string Protect(UserId owner, KidsnoteLoginRequest login)
    {
        using var cipher = CreateCipher();
        var data = JsonSerializer.SerializeToUtf8Bytes(login);
        var nonce = RandomNumberGenerator.GetBytes(12);
        var tag = new byte[16];
        var ciphertext = new byte[data.Length];
        cipher.Encrypt(nonce, data, ciphertext, tag, Encoding.UTF8.GetBytes(owner.Id));
        return Convert.ToBase64String([.. nonce, .. tag, .. ciphertext]);
    }

    public KidsnoteLoginRequest Open(UserId owner, string encrypted)
    {
        using var cipher = CreateCipher();
        var data = Convert.FromBase64String(encrypted);
        var plaintext = new byte[data.Length - 28];
        cipher.Decrypt(data.AsSpan(0, 12), data.AsSpan(28), data.AsSpan(12, 16), plaintext, Encoding.UTF8.GetBytes(owner.Id));
        return JsonSerializer.Deserialize<KidsnoteLoginRequest>(plaintext)!;
    }
}
