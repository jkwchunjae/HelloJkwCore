using Common;
using Common.Extensions;
using Common.FileSystem;
using Dropbox.Api;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloJkwCore.Authentication
{
    public class AuthUtil
    {
        private List<OAuthOption> _oauthOptions;

        public AuthUtil(IFileSystem fs)
        {
            var task = fs.ReadJsonAsync<List<OAuthOption>>(PathType.OAuthOption.GetPath());
            task.Wait();
            _oauthOptions = task.Result;
        }

        public OAuthOption GetAuthOption(AuthProvider provider)
        {
            return _oauthOptions
                ?.FirstOrDefault(x => x.Provider == provider);
        }
    }
}
