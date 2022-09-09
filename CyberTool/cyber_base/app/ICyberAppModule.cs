using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_base.app
{
    public interface ICyberAppModule
    {
        public void OnModuleStart();

        public void OnModuleDestroy();
    }
}
