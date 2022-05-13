using cyber_base.utils.async_task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace cyber_base.app
{
    public interface ICyberApplication
    {
        Application? CyberApp { get; }

        void OpenWaitingTaskBox(string content
            , string title
            , Func<object, CancellationToken, Task<AsyncTaskResult>> asyncTask
            , Func<bool> canExecute = null
            , Action<object, AsyncTaskResult> callback = null
            , long delayTime = 0);
    }
}
