using Common;
using JkwExtensions;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloJkwCore.User
{
    public partial class UserStore : IQueryableUserStore<AppUser>
    {
        public IQueryable<AppUser> Users
        {
            get
            {
                var userFilesTask = _fs.GetFilesAsync(_usersPath, ".json");
                userFilesTask.Wait();
                var userFiles = userFilesTask.Result;

                var usersTask = userFiles.Select(async file => await _fs.ReadJsonAsync<AppUser>(path => path.UserFilePathByFileName(file)))
                    .WhenAll();
                usersTask.Wait();

                return usersTask.Result.AsQueryable();
            }
        }
    }
}
