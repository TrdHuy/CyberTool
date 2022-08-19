using cyber_extension.dll_base.extension;
using cyber_core.@base.module;
using cyber_core.utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_core.plugins
{
    public class CyberPluginsManager : ICyberModule
    {
        private static string PLUGIN_FOLDER_PATH { get; } = System.AppDomain.CurrentDomain.BaseDirectory + "plugins";
        private static string CYBER_EXTENSION_MODULE_PATH { get; } = System.AppDomain.CurrentDomain.BaseDirectory + "plugins\\cyber_extension.dll";
        public Collection<ICyberExtension> Plugins { get; }

        public static CyberPluginsManager Current
        {
            get
            {
                return CyberToolModuleManager.CPM_Instance;
            }
        }
        private CyberPluginsManager()
        {
            Plugins = new Collection<ICyberExtension>();
        }

        public void OnModuleInit()
        {
            LoadExternalPlugin();
        }

        public void OnModuleStart()
        {
        }

        public void OnIFaceWindowShowed()
        {
        }

        public void LoadExternalPlugin()
        {
            Plugins.Clear();
            if (!Directory.Exists(PLUGIN_FOLDER_PATH))
            {
                Directory.CreateDirectory(PLUGIN_FOLDER_PATH);
            }

            DirectoryInfo dInfo = new DirectoryInfo(PLUGIN_FOLDER_PATH);
            FileInfo[] pluginFiles = dInfo.GetFiles("*.dll");

            foreach (var fileInfo in pluginFiles)
            {
                if (fileInfo.FullName != CYBER_EXTENSION_MODULE_PATH)
                {
                    var plugin = System.Reflection.Assembly.LoadFile(fileInfo.FullName);

                    var types = plugin.GetTypes().Where(eT => eT.GetInterfaces().Contains(typeof(ICyberExtension)));
                    foreach (var t in types)
                    {
                        var instance = Activator.CreateInstance(t) as ICyberExtension;

                        if (instance != null)
                        {
                            Plugins.Add(instance);
                            break;
                        }

                    }
                }

            }
        }

    }
}
