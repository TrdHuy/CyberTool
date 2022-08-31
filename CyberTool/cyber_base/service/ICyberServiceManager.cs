using cyber_base.app;
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
    }
}
