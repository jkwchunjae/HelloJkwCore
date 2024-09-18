using Dropbox.Api;
using Dropbox.Api.Files;
using static Dropbox.Api.Files.WriteMode;

namespace Common;

public static class DropboxExtensions
{
    public static DropboxClient GetDropboxClient(DropboxOption dropboxOption)
    {
        return new DropboxClient(
            dropboxOption.RefreshToken,
            dropboxOption.ClientId,
            dropboxOption.ClientSecret);
    }
}