using Dropbox.Api;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Extensions
{
    public static class DropboxExtensions
    {
        public static async Task<T> ReadJsonAsync<T>(this DropboxClient client, string path)
        {
            var fileMetadata = await client.Files.DownloadAsync(path);
            var fileText = await fileMetadata.GetContentAsStringAsync();
            return Json.Deserialize<T>(fileText);
        }
    }
}
