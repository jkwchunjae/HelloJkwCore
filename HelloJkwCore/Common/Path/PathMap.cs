using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class PathMap
    {
        public Dictionary<string, string> Default { get; set; } = new();
        public Dictionary<string, string> Dropbox { get; set; } = new();
        public Dictionary<string, string> Azure { get; set; } = new();
        public Dictionary<string, string> Local { get; set; } = new();

        public Dictionary<string, string> this[FileSystemType type]
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
