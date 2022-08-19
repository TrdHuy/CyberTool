using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Base.Utils
{
    public interface IDestroyable
    {
        /// <summary>
        /// destroy callback method when an object is unloaded or destroyed
        /// </summary>
        void OnDestroy();

    }
}
