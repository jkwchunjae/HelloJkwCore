using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Common.Extensions
{
    public static class ConfigurationPathExtensions
    {
        private static Dictionary<PathType, string> _pathConfig = null;

        public static void SetPathConfig(Dictionary<PathType, string> config)
        {
            _pathConfig = config;
        }

        private static List<(string Prefix, Func<string> PathFunc)> _pathPrefix
            = new List<(string Prefix, Func<string> PathFunc)>
            {
                (Prefix: "dropbox://", PathFunc: DropboxRootPath),
            };

        private static string DropboxRootPath()
            => GetPath(PathType.DropboxRoot);

        public static string GetPath(this PathType pathType)
        {
            if (_pathConfig is null)
                throw new NullReferenceException("Set _pathConfig. Use SetPathConfing method.");

            if (!_pathConfig.ContainsKey(pathType))
                return null;

            var path = _pathConfig[pathType];

            if (_pathPrefix.Any(x => path.StartsWith(x.Prefix)))
            {
                var prefixData = _pathPrefix.First(x => path.StartsWith(x.Prefix));
                var prefix = prefixData.Prefix;
                var prefixPath = prefixData.PathFunc();

                return $"{prefixPath}/{path.Substring(prefix.Length)}";
            }
            else
            {
                return path;
            }
        }
    }
}
