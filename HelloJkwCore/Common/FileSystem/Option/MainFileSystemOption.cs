using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common;

public class MainFileSystemOption
{
    public FileSystemType MainFileSystem { get; set; }
    public FileSystemType BackupFileSystem { get; set; }
    public bool UseBackup { get; set; }
}