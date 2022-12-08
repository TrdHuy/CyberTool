using progtroll.@base.module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progtroll.implement.module
{
    internal abstract class BasePublisherModule : IPublisherModule
    {
        public virtual void OnDestroy()
        {
        }

        public virtual void OnModuleStart()
        {
        }

        public virtual void OnViewInstantiated()
        {
        }
    }
}
