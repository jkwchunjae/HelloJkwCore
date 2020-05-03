using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HelloJkwTests
{
    static class UnitTest_Base
    {
        private static readonly string ConfigurationRootPath;

        static UnitTest_Base()
        {
            // D:\jkw\major\GitHub\HelloJkwCore\HelloJkwCore\HelloJkwTests\bin\Debug\netcoreapp3.1

            var currentDirInfo = new DirectoryInfo(Environment.CurrentDirectory);
            ConfigurationRootPath = Path.Combine(currentDirInfo.Parent.Parent.Parent.Parent.FullName, "HelloJkwServer");
        }

        public static IConfiguration GetIConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(ConfigurationRootPath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
