using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_installer.@base
{
    internal abstract class BaseCyberInstallerModule : ICyberInstallerModule
    {
        public virtual void OnModuleCreate()
        {
        }

        public virtual void OnModuleStart()
        {
        }

        public virtual void OnMainWindowShowed()
        {
        }

        public virtual void OnModuleDestroy()
        {
        }
    }
}
