using cyber_base.definition;
using cyber_base.utils.async_task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace cyber_base.app
{
    public interface ICyberApplication
    {
        Application CyberApp { get; }

        CyberContactMessage OpenWaitingTaskBox(string content
            , string title
            , Func<object, CancellationToken, Task<AsyncTaskResult>> asyncTask
            , Func<bool> canExecute = null
            , Action<object, AsyncTaskResult> callback = null
            , long delayTime = 0);

        CyberContactMessage ShowWaringBox(string warning
            , bool isDialog = true);

        string OpenFileChooserDialogWindow(string title = "Choose a log file"
            , string filter = "Text files (*.txt)|*.txt|Log files (*.log)|*.log");

        void ShowPopupCControl(ContentControl cc
           , UIElement opener
           , CyberOwner ownerWindow = CyberOwner.Default
           , double width = 500
           , double height = 400
           , object dataContext = null
           , Action<object> windowShowedCallback = null
           , string title = "Floating window");
    }
}
