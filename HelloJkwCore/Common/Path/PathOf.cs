using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JkwExtensions;

namespace Common
{
    public partial class PathOf
    {
        private readonly Dictionary<PathType, string> _pathDic;

        public PathOf(PathOption option, FileSystemType fsType)
        {
            _pathDic = option.Default.Select(x => x.Key)
                .Concat(option[fsType].Select(x => x.Key))
                .Distinct()
                .Select(key => new
                {
                    Key = key,
                    Value = option[fsType].ContainsKey(key) ? option[fsType][key] : option.Default[key],
                })
                .ToDictionary(x => x.Key, x => x.Value);
        }

        public string GetPath(PathType pathType)
        {
            return _pathDic[pathType];
        }

        #region User

        public string UserFilePathByUserId(string userId)
        {
            return $"{GetPath(PathType.UsersPath)}/user.{userId}.json".ToLower();
        }

        public string UserFilePathByFileName(string fileName)
        {
            return $"{GetPath(PathType.UsersPath)}/{fileName}";
        }

        #endregion
    }
}
