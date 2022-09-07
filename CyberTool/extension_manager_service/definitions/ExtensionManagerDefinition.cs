using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace extension_manager_service.definitions
{
    internal class ExtensionManagerDefinition
    {
        public const string EXTENSION_PAGE_URI_ORIGINAL_STRING = "/extension_manager_service;component/views/pages/ExtensionManager.xaml";
        public const string SERVICE_KEY = "extension_manager";
        public static readonly long EXTENSION_PAGE_LOADING_DELAY_TIME = 500;
        public const string SERVICE_TAG = "ExtensionManager";

        public const string EXTENSION_PAGE_HEADER_GEOMETRY_DATA = "M17.37,29.73H0V12.37H17.37Zm0,2.45H0V49.54H17.37Zm19.8,0H19.81V49.54H37.17ZM45,23.37l1-1a12.51,12.51,0,0,0,0-17.7l-1-1a12.52,12.52,0,0,0-17.71,0l-1,1a12.51,12.51,0,0,0,0,17.7l1,1A12.52,12.52,0,0,0,45,23.37Z";

    }
    
    internal class ExtensionManagerKeyFeatureTag
    {
        public const string KEY_TAG_EMS_INSTALL_PLUGIN_FEATURE = "KEY_TAG_EMS_INSTALL_PLUGIN_FEATURE";
        public const string KEY_TAG_EMS_UNINSTALL_PLUGIN_FEATURE = "KEY_TAG_EMS_UNINSTALL_PLUGIN_FEATURE";

    }

}
