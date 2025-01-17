﻿using cyber_base.app;
using cyber_base.extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace cyber_base.service
{
    public interface ICyberServiceManager 
    {
        public ICyberApplication App { get; }

        public string GetServicesBaseFolderLocation();

        public string GetPluginsBaseFolderLocation();

        public void RegisterExtensionAsCyberService(ICyberExtension cyberExtension);

        public void UnregisterExtensionAsCyberService(ICyberExtension cyberExtension);
    }
}
