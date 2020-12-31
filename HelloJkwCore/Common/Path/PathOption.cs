using Common.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class PathOption
    {
        public Dictionary<PathType, string> Default { get; set; } = new();
        public Dictionary<PathType, string> Dropbox { get; set; } = new();
        public Dictionary<PathType, string> Azure { get; set; } = new();
        public Dictionary<PathType, string> Local { get; set; } = new();

        public Dictionary<PathType, string> this[FileSystemType type]
        {
            get
            {
                switch (type)
                {
                    case FileSystemType.Dropbox:
                        return Dropbox;
                    case FileSystemType.Azure:
                        return Azure;
                    case FileSystemType.Local:
                        return Local;
                    default:
                        return Default;
                }
            }
        }
    }
}
