using Common;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloJkwCore
{
    public class CoreOption
    {
        public FileSystemSelectOption AuthFileSystem { get; set; }
        public FileSystemSelectOption UserStoreFileSystem { get; set; }
        public PathMap Path { get; set; }

        public static CoreOption Create(IConfiguration configuration)
        {
            var option = new CoreOption();
            configuration.GetSection("HelloJkw").Bind(option);
            return option;
        }
    }
}
