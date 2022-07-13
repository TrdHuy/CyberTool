using cyber_base.async_task;
using cyber_base.definition;
using cyber_base.implement.async_task;
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
            , Func<object, AsyncTaskResult, CancellationTokenSource, Task<AsyncTaskResult>> asyncTask
            , Func<object, bool>? canExecute = null
            , Func<object, AsyncTaskResult, Task<AsyncTaskResult>>? callback = null
            , ulong delayTime = 0
            , ulong estimatedTime = 0
            , string taskName = "");

        CyberContactMessage OpenMultiTaskBox(string title
            , MultiAsyncTask task
            , bool isCancelable = true
            , Action<object>? multiTaskDoneCallback = null
            , bool isUseMultiTaskReport = true);


        CyberContactMessage ShowWaringBox(string warning
            , bool isDialog = true);

        CyberContactMessage ShowYesNoQuestionBox(string question
            , bool isDialog = true);

        string OpenFileChooserDialogWindow(string title = "Choose a log file"
            , string filter = "Text files (*.txt)|*.txt|Log files (*.log)|*.log");

        void ShowPopupCControl(ContentControl cc
           , UIElement opener
           , CyberOwner ownerWindow = CyberOwner.Default
           , double width = 500
           , double height = 400
           , object? dataContext = null
           , Action<object>? windowShowedCallback = null
           , string title = "Floating window");

        string OpenFolderChooserDialogWindow();

    }
}
