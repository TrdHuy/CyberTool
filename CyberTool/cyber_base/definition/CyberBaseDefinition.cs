using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_base.definition
{
    public static class CyberBaseDefinition
    {
        public static string DEFAULT_CYBER_WINDOW_STYLE_KEY = "DefaultCyberWindowStyle";

        public static string QUESTION_ICON_GEOMETRY_RESOURCE_KEY = "QUESTION_ICON_GEOMETRY_RESOURCE";
        public static string SUCCESS_ICON_GEOMETRY_RESOURCE_KEY = "SUCCESS_ICON_GEOMETRY_RESOURCE";

    }

    public enum CyberOwner
    {
        Default = 0,
        ServiceManager = 1
    }

    public enum CyberContactMessage
    {
        None = 0,
        Yes = 1,
        No = 2,
        Cancel = 3,
        Continue = 4,
        Done = 5
    }

    public enum CyberIWinOwnerType
    {
        Default = 0,
        Parents = 1,
    }
}
