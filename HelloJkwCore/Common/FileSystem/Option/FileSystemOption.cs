using Common.Dropbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class FileSystemOption
    {
        public MainFileSystemOption MainFileSystem { get; set; }
        public DropboxOption Dropbox { get; set; }
    }
}
