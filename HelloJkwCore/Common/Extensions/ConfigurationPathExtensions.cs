﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Common.Extensions
{
    public static class ConfigurationPathExtensions
    {
        private static IConfiguration _configuration;

        public static void SetConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private static List<(string Prefix, Func<IConfiguration, string> PathFunc)> _pathPrefix
            = new List<(string Prefix, Func<IConfiguration, string> PathFunc)>
            {
                (Prefix: "dropbox://", PathFunc: DropboxRootPath),
            };

        private static string DropboxRootPath(this IConfiguration configuration)
            => configuration.GetPath(PathType.DropboxRoot);

        public static string GetPath(this IConfiguration configuration, PathType pathType)
        {
            var path = configuration[$"Path:{pathType}"];

            if (_pathPrefix.Any(x => path.StartsWith(x.Prefix)))
            {
                var prefixData = _pathPrefix.First(x => path.StartsWith(x.Prefix));
                var prefix = prefixData.Prefix;
                var prefixPath = prefixData.PathFunc(configuration);

                return Path.Combine(prefixPath, path.Substring(prefix.Length));
            }
            else
            {
                return path;
            }
        }

        public static string GetPath(this PathType pathType)
        {
            return _configuration?.GetPath(pathType);
        }
    }
}
