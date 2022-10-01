using cyber_installer.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_installer.@base.modules
{
    internal interface IUserDataManager
    {
        public UserData CurrentUserData { get; }
        public void UpdateUserData(UserData newData);
        public Task<bool> WaitForImportUserDataTask(int timeOut);

    }
}
