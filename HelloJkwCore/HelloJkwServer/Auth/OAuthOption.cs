using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloJkwServer.Auth
{
    public class OAuthOption
    {
        public string Provider { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
