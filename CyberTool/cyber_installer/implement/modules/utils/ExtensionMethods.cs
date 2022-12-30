using cyber_installer.definitions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static cyber_installer.definitions.CyberInstallerDefinition;

namespace cyber_installer.implement.modules.utils
{
    public static class ExtensionMethods
    {
        public static string GetDescription(this ManageableTaskKeyDefinition taskTypeKey)
        {
            var attributes =  taskTypeKey
              .GetType()
              .GetField(taskTypeKey.ToString())?
              .GetCustomAttributes(typeof(ManageableTaskInfoAttribute), false) as ManageableTaskInfoAttribute[];
            return attributes?.Length > 0 ? attributes[0].Description : string.Empty;
        }

        public static string GetName(this ManageableTaskKeyDefinition taskTypeKey)
        {
            var attributes = taskTypeKey
              .GetType()
              .GetField(taskTypeKey.ToString())?
              .GetCustomAttributes(typeof(ManageableTaskInfoAttribute), false) as ManageableTaskInfoAttribute[];
            return attributes?.Length > 0 ? attributes[0].Name : string.Empty;
        }
    }
}
