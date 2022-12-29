using cyber_base.definition;
using cyber_base.implement.async_task;
using cyber_base.implement.views.cyber_window.cyber_imes;
using cyber_base.implement.views.cyber_window.cyber_istand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows;
using cyber_installer.view.window;
using cyber_base.async_task;
using cyber_installer.model;

namespace cyber_installer.implement.app_support_modules
{
    public class WindowDirector
    {
        private CyberInstallerWindow? _cyberInstallerWindow;

        public CyberInstallerWindow CyberInstallerWindow
        {
            get
            {
                if (_cyberInstallerWindow == null)
                {
                    _cyberInstallerWindow = new CyberInstallerWindow();
                }
                return _cyberInstallerWindow;
            }
        }

        public WindowDirector()
        {
        }

        public void Init()
        {
            _cyberInstallerWindow = new CyberInstallerWindow();
        }


        public CyberContactMessage ShowErrorBox(string error)
        {
            CyberIMesWindow mesBox = new CyberIMesWindow(
                "Error",
                Application.Current.Resources[CyberBaseDefinition.QUESTION_ICON_GEOMETRY_RESOURCE_KEY] as string ?? "",
                error,
                "",
                "",
                "Continue",
                "",
                _cyberInstallerWindow
                );
            return ConvertToContactMessage(mesBox.ShowDialog());
        }

        public CyberContactMessage ShowYesNoQuestionBox(string question, bool isDialog)
        {
            CyberIMesWindow mesBox = new CyberIMesWindow(
                title: "Question",
                pathIcon: Application.Current.Resources[CyberBaseDefinition.QUESTION_ICON_GEOMETRY_RESOURCE_KEY] as string ?? "",
                content: question,
                yesBtnContent: "Yes",
                noBtnContent: "No",
                continueBtnContent: "",
                cancelBtnContent: "",
                owner: _cyberInstallerWindow
                );
            CyberIMesBoxResult res = CyberIMesBoxResult.Continue;
            if (isDialog)
            {
                res = mesBox.ShowDialog();
            }
            else
            {
                mesBox.Show();
            }
            return ConvertToContactMessage(res);
        }

        public CyberContactMessage ShowWarningBox(string warning, bool isDialog)
        {
            CyberIMesWindow mesBox = new CyberIMesWindow(
                "Warning",
                Application.Current.Resources[CyberBaseDefinition.QUESTION_ICON_GEOMETRY_RESOURCE_KEY] as string ?? "",
                warning,
                "",
                "",
                "Continue",
                "",
                _cyberInstallerWindow
                );
            CyberIMesBoxResult res = CyberIMesBoxResult.Continue;
            if (isDialog)
            {
                res = mesBox.ShowDialog();
            }
            else
            {
                mesBox.Show();
            }
            return ConvertToContactMessage(res);
        }

        public CyberContactMessage ShowSuccessBox(string message, bool isDialog)
        {
            CyberIMesWindow mesBox = new CyberIMesWindow(
                title: "Info",
                pathIcon: Application.Current.Resources[CyberBaseDefinition.SUCCESS_ICON_GEOMETRY_RESOURCE_KEY] as string ?? "",
                content: message,
                yesBtnContent: "",
                noBtnContent: "",
                continueBtnContent: "Continue",
                cancelBtnContent: "",
                owner: _cyberInstallerWindow);
            CyberIMesBoxResult res = CyberIMesBoxResult.Continue;
            if (isDialog)
            {
                res = mesBox.ShowDialog();
            }
            else
            {
                mesBox.Show();
            }
            return ConvertToContactMessage(res);
        }

        public void ShowMainWindow()
        {
            _cyberInstallerWindow?.Show();
        }

        public DestinationFolderSelectionWindow ShowDestinationFolderWindow(ToolVO toolVO)
        {
            var destinationFolderWindow = new DestinationFolderSelectionWindow(toolVO)
            {
                Owner = _cyberInstallerWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            var res = destinationFolderWindow.Show();
            return destinationFolderWindow;
        }

        public CyberContactMessage OpenWaitingTaskBox(string content
            , string title
            , Func<object, AsyncTaskResult, CancellationTokenSource, Task<AsyncTaskResult>> asyncTask
            , Func<object, bool>? canExecute = null
            , Func<object, AsyncTaskResult, Task<AsyncTaskResult>>? callback = null
            , ulong delayTime = 0
            , ulong estimatedTime = 0
            , string taskName = "")
        {
            var newWaitingBox = new CyberIStandWindow(content
                , title
                , asyncTask
                , canExecute
                , callback
                , delayTime
                , taskName
                , estimatedTime
                , _cyberInstallerWindow);

            var message = newWaitingBox.Show();

            return ConvertToContactMessage(message);
        }

        public CyberContactMessage OpenMultiTaskBox(string title
            , MultiAsyncTask tasks
            , bool isCancelable = true
            , Action<object>? multiTaskDoneCallback = null
            , bool isUseMultiTaskReport = true)
        {
            var newWaitingBox = new CyberIStandWindow(title
                , tasks
                , _cyberInstallerWindow
                , isCancelable
                , multiTaskDoneCallback
                , isUseMultiTaskReport);

            var message = newWaitingBox.Show();

            return ConvertToContactMessage(message);
        }

        private CyberContactMessage ConvertToContactMessage(object mes)
        {
            switch (mes)
            {
                case CyberIMesBoxResult.None:
                case CyberIStandBoxResult.None:
                    return CyberContactMessage.None;
                case CyberIMesBoxResult.Cancel:
                case CyberIStandBoxResult.Cancel:
                    return CyberContactMessage.Cancel;
                case CyberIStandBoxResult.Done:
                    return CyberContactMessage.Done;
                case CyberIMesBoxResult.Yes:
                    return CyberContactMessage.Yes;
                case CyberIMesBoxResult.No:
                    return CyberContactMessage.No;
                case CyberIMesBoxResult.Continue:
                    return CyberContactMessage.Continue;
            }

            return CyberContactMessage.None;
        }
    }

}
