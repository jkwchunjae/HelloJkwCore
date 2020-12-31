using Common.FileSystem;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloJkwCore
{
    public class CoreOption
    {
        public FileSystemType UserStoreFileSystem { get; set; }

        public static CoreOption Create(IConfiguration configuration)
        {
            var option = new CoreOption();
            configuration.GetSection("HelloJkw").Bind(option);
            return option;
        }
    }
}
