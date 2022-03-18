using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Base.ViewModel
{
    internal interface IPageViewModel
    {
        void OnUnloaded();

        void OnLoaded();
    }
}
