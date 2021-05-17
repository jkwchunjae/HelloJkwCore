using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSuFc
{
    public partial class SuFcService : ISuFcService
    {
        IFileSystem _fs;

        public SuFcService(
            SuFcOption option,
            IFileSystemService fsService)
        {
            _fs = fsService.GetFileSystem(option.FileSystemSelect);

            InitTeamMaker();
        }
    }
}
