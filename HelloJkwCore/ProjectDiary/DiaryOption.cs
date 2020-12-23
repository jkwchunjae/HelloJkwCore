using Common.FileSystem;

namespace ProjectDiary
{
    public class DiaryOption
    {
        public FileSystemType MainFileSystem { get; set; }
        public FileSystemType BackupFileSystem { get; set; }
        public bool UseBackup { get; set; }
    }
}
