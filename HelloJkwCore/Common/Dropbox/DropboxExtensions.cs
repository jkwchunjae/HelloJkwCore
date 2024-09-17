using Dropbox.Api;
using Dropbox.Api.Files;
using static Dropbox.Api.Files.WriteMode;

namespace Common;

public static class DropboxExtensions
{
    private static readonly Encoding _defaultEncoding = new UTF8Encoding(false);
    public static DropboxClient GetDropboxClient(DropboxOption dropboxOption)
    {
        return new DropboxClient(
            dropboxOption.RefreshToken,
            dropboxOption.ClientId,
            dropboxOption.ClientSecret);
    }

    public static async Task<T> ReadJsonAsync<T>(this DropboxClient client, string path)
    {
        var fileText = await client.ReadTextAsync(path);
        return Json.Deserialize<T>(fileText);
    }
    public static async Task<string> ReadTextAsync(this DropboxClient client, string path)
    {
        var fileMetadata = await client.Files.DownloadAsync(path);
        var fileText = await fileMetadata.GetContentAsStringAsync();
        return fileText;
    }

    public static async Task<FileMetadata> WriteTextAsync(this DropboxClient client, string path, string text, Encoding? encoding = null)
    {
        encoding ??= _defaultEncoding;

        var bytes = encoding.GetBytes(text);
        using (var stream = new MemoryStream(bytes))
        {
            var uploadArgs = new UploadArg(path, mode: Overwrite.Instance);
            return await client.Files.UploadAsync(uploadArgs, stream);
        }
    }

    public static async Task<FileMetadata> WriteJsonAsync<T>(this DropboxClient client, string path, T obj, Encoding? encoding = null)
    {
        var text = Json.Serialize(obj);
        return await client.WriteTextAsync(path, text, encoding);
    }
}