using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_base.extension
{
    public interface ICyberExtension
    {
        void OnPluginInstalled(ICyberExtensionManager extensionManager);

        void OnPluginUninstalled(ICyberExtensionManager extensionManager);

        void OnPluginStart(ICyberExtensionManager extensionManager);
    }
}
