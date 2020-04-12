using Common;
using HelloJkwServer.Misc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HelloJkwServer.Auth
{
    public class AuthUtils
    {
        private IConfiguration _configuration;
        private List<OAuthOption> _oauthOptions;

        public AuthUtils(IConfiguration configuration)
        {
            _configuration = configuration;
            _oauthOptions = LoadOAuthOptions();
        }

        private List<OAuthOption> LoadOAuthOptions()
        {
            var path = _configuration.GetPath(PathOf.OAuthOption);
            var jsonText = File.ReadAllText(path, Encoding.UTF8);
            var options = JsonConvert.DeserializeObject<List<OAuthOption>>(jsonText);
            return options;
        }

        public OAuthOption GetOAuthOption(string provider)
        {
            return _oauthOptions.FirstOrDefault(x => x.Provider == provider);
        }
    }
}
