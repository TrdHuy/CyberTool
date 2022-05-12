using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace cyber_base.app
{
    public interface ICyberApplication
    {
        Application? Current { get; }

        void OpenWaitingTaskBox();
    }
}
