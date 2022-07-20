using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Base.ViewModel
{
    public interface IPageViewModel
    {
        /// <summary>
        /// Occur when the page was unloaded
        /// </summary>
        /// <returns> 
        ///     true if should remove all the child models in cache
        ///     false if not remove all child models in cache
        /// </returns>
        bool OnUnloaded();

        /// <summary>
        /// Occur when the page was loaded
        /// </summary>
        void OnLoaded();
    }
}
