using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_base.utils
{
    public interface IDestroyable
    {
        /// <summary>
        /// destroy callback method when an object is unloaded or destroyed
        /// </summary>
        void OnDestroy();

    }
}
