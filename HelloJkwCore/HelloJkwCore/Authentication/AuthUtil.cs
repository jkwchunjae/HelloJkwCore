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
            var task = fs.ReadJsonAsync<List<OAuthOption>>(PathOf.Get(PathType.OAuthOption));
            task.Wait();
            _oauthOptions = task.Result;
        }

        //private List<OAuthOption> LoadFromConfiguration()
        //{
        //    var googleAuthOption = new OAuthOption
        //    {
        //        Provider = AuthProvider.Google,
        //        ClientId = _configuration["Auth:Google:ClientId"],
        //        ClientSecret = _configuration["Auth:Google:ClientSecret"],
        //        Callback = _configuration["Auth:Google:Callback"],
        //    };
        //    var kakaoAuthOption = new OAuthOption
        //    {
        //        Provider = AuthProvider.KakaoTalk,
        //        ClientId = _configuration["Auth:Kakao:ClientId"],
        //        ClientSecret = _configuration["Auth:Kakao:ClientSecret"],
        //        Callback = _configuration["Auth:Kakao:Callback"],
        //    };

        //    return new List<OAuthOption>
        //    {
        //        googleAuthOption,
        //        kakaoAuthOption,
        //    };
        //}

        public OAuthOption GetAuthOption(AuthProvider provider)
        {
            return _oauthOptions
                ?.FirstOrDefault(x => x.Provider == provider);
        }
    }
}
