using Common.FileSystem;
using Dropbox.Api;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dropbox
{
    public static class DropboxHelper
    {
        public static DropboxClient GetDropboxClient(DropboxOption dropboxOption)
        {
            return new DropboxClient(
                dropboxOption.RefreshToken,
                dropboxOption.ClientId,
                dropboxOption.ClientSecret);
        }
    }
}
