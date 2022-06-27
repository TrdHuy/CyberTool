using honeyboard_release_service.@base.module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace honeyboard_release_service.implement.module
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
