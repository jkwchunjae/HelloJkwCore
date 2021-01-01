using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public partial class PathOf
    {
        private readonly Dictionary<PathType, string> _pathDic;

        public PathOf(PathOption option, FileSystemType fsType)
        {
            _pathDic = option.Default
                .Select(x => new { x.Key, x.Value })
                .Select(x => new
                {
                    x.Key,
                    Value = option[fsType].ContainsKey(x.Key) ? option[fsType][x.Key] : x.Value,
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
