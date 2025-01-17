﻿using cyber_base.async_task;
using cyber_base.definition;
using cyber_base.implement.async_task;
using cyber_base.implement.views.cyber_window.cyber_imes;
using cyber_base.implement.views.cyber_window.cyber_istand;
using cyber_core.definitions;
using cyber_core.windows.cyber_iface.views;
using cyber_core.windows.cyber_ipop.views;
using cyber_core.windows.cyber_itext.views;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace cyber_core.windows
{

    public class WindowDirector
    {
        private Dictionary<ContentControl, CyberIPopWindow> _IPopWindowMap;
        private CyberIFaceWindow? _IFaceWindow;
        private CyberIPopWindow? _IPopWindow;

        public CyberIFaceWindow IFaceWindow
        {
            get
            {
                if (_IFaceWindow == null)
                {
                    _IFaceWindow = new CyberIFaceWindow();
                }
                return _IFaceWindow;
            }
            set
            {
                _IFaceWindow = value;
            }
        }

        public WindowDirector()
        {
            _IPopWindowMap = new Dictionary<ContentControl, CyberIPopWindow>();
        }


        public void ShowCyberIFace()
        {
            IFaceWindow.Show();
        }

        public CyberIMesBoxResult ShowErrorBox(string error)
        {
            CyberIMesWindow mesBox = new CyberIMesWindow(
                "Error",
                Application.Current.Resources[CyberBaseDefinition.QUESTION_ICON_GEOMETRY_RESOURCE_KEY] as string ?? "",
                error,
                "",
                "",
                "Continue",
                "",
                IFaceWindow
                );
            return mesBox.ShowDialog();
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
                owner: IFaceWindow
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
                _IFaceWindow
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
                , IFaceWindow);

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
                , IFaceWindow
                , isCancelable
                , multiTaskDoneCallback
                , isUseMultiTaskReport);

            var message = newWaitingBox.Show();

            return ConvertToContactMessage(message);
        }

        public void ShowPopupCustomControlWindow(ContentControl cc
            , UIElement opener
            , CyberOwnerWindow ownerWindow = CyberOwnerWindow.Default
            , double width = 500
            , double height = 400
            , object? dataContext = null
            , Action<object>? windowShowedCallback = null
            , string title = "Floating window")
        {
            if (_IPopWindowMap.ContainsKey(cc))
            {
                _IPopWindow = _IPopWindowMap[cc];
                if (_IPopWindow != null && _IPopWindow.WindowState == WindowState.Minimized)
                {
                    _IPopWindow.WindowState = WindowState.Normal;
                }
                return;
            }

            switch (ownerWindow)
            {
                case CyberOwnerWindow.Default:
                    _IPopWindow = new CyberIPopWindow(opener, null);
                    _IPopWindow.DataContext = dataContext;
                    _IPopWindow.Title = title;
                    break;
                case CyberOwnerWindow.CyberIFace:
                    _IPopWindow = new CyberIPopWindow(opener, IFaceWindow);
                    _IPopWindow.DataContext = dataContext;
                    _IPopWindow.Title = title;
                    break;
            }

            if (_IPopWindow != null)
            {
                _IPopWindow.Height = height;
                _IPopWindow.Width = width;

                _IPopWindowMap.Add(cc, _IPopWindow);

                StartDispandCCAnim(cc, _IPopWindow, 200, windowShowedCallback);
            }

        }

        public void ShowPopupUserControlWindow(UserControl uc
           , CyberOwnerWindow ownerWindow = CyberOwnerWindow.Default
           , double width = 500
           , double height = 400
           , Action<object>? windowShowedCallback = null
           , string title = "Floating window")
        {
            if (_IPopWindowMap.ContainsKey(uc))
            {
                _IPopWindow = _IPopWindowMap[uc];
                if (_IPopWindow != null && _IPopWindow.WindowState == WindowState.Minimized)
                {
                    _IPopWindow.WindowState = WindowState.Normal;
                }
                return;
            }

            var owner = IFaceWindow;
            switch (ownerWindow)
            {
                case CyberOwnerWindow.Default:
                    owner = null;
                    break;
                case CyberOwnerWindow.CyberIFace:
                    break;
            }
            var userControlWindow = new CyberIPopWindow(null, owner);
            userControlWindow.Title = title;
            userControlWindow.Content = uc;

            if (userControlWindow != null)
            {
                _IPopWindow = userControlWindow;
                userControlWindow.Height = height;
                userControlWindow.Width = width;

                _IPopWindowMap.Add(uc, userControlWindow);
                userControlWindow.Closed += (s1, e1) =>
                {
                    _IPopWindowMap.Remove(uc);
                };
                _IPopWindow.Show();
                windowShowedCallback?.Invoke(_IPopWindow);

            }
        }

        public string OpenSaveLogFileDialogWindow()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Text files (*.txt)|*.txt|Log files (*.log)|*.log";
            saveFileDialog1.Title = "Save an log File";
            saveFileDialog1.FileName = "Log_" + DateTime.Now.ToString("MM-dd-yyyy_HH-mm-ss");

            saveFileDialog1.ShowDialog();
            return saveFileDialog1.FileName;
        }

        public string OpenFileChooserDialogWindow(string title = "Choose a log file", string filter = "Text files (*.txt)|*.txt|Log files (*.log)|*.log")
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = filter;
            openFileDialog.Title = title;
            if (openFileDialog.ShowDialog() == true)
                return openFileDialog.FileName;
            return "";
        }

        public string OpenFolderChooserDialogWindow()
        {
            using (var fbd = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = fbd.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    return fbd.SelectedPath;
                }
            }
            return "";
        }

        public string OpenEditTextDialogWindow(string oldText, bool isMultiLine)
        {
            var editTextWindow = new CyberITextWindow(oldText, isMultiLine);
            editTextWindow.Title = "Edit box";
            editTextWindow.Owner = _IFaceWindow;
            editTextWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            var newText = editTextWindow.ShowDialog();
            return newText;
        }

        private void StartDispandCCAnim(ContentControl cc
            , CyberIPopWindow floatWindow
            , int animTime
            , Action<object>? windowShowedCallback = null)
        {
            Storyboard dispand = new Storyboard();
            DoubleAnimation scaleXAnim = new DoubleAnimation();
            Storyboard.SetTargetName(scaleXAnim, "CC_ScaleTransform");
            Storyboard.SetTargetProperty(scaleXAnim, new PropertyPath(ScaleTransform.ScaleXProperty));
            scaleXAnim.From = 1.0d;
            scaleXAnim.To = 3d;
            scaleXAnim.Duration = TimeSpan.FromMilliseconds(animTime);

            DoubleAnimation scaleYAnim = new DoubleAnimation();
            Storyboard.SetTargetName(scaleYAnim, "CC_ScaleTransform");
            Storyboard.SetTargetProperty(scaleYAnim, new PropertyPath(ScaleTransform.ScaleYProperty));
            scaleYAnim.From = 1.0d;
            scaleYAnim.To = 3d;
            scaleYAnim.Duration = TimeSpan.FromMilliseconds(animTime);
            dispand.Children.Add(scaleYAnim);
            dispand.Children.Add(scaleXAnim);

            dispand.Completed += (s, e) =>
            {
                var content = cc.Content;
                cc.Content = null;
                floatWindow.Content = content;
                floatWindow.Closed += (s1, e1) =>
                {
                    cc.Content = content;
                    StartExpandCCAnim(cc, animTime);
                    _IPopWindowMap.Remove(cc);
                };
                floatWindow.Show();

                windowShowedCallback?.Invoke(floatWindow);
            };

            dispand.Begin(cc);
        }

        private void StartExpandCCAnim(ContentControl cc, int animTime)
        {
            Storyboard expandSB = new Storyboard();
            DoubleAnimation scaleXAnim = new DoubleAnimation();
            Storyboard.SetTargetName(scaleXAnim, "CC_ScaleTransform");
            Storyboard.SetTargetProperty(scaleXAnim, new PropertyPath(ScaleTransform.ScaleXProperty));
            scaleXAnim.From = 3d;
            scaleXAnim.To = 1.0d;
            scaleXAnim.Duration = TimeSpan.FromMilliseconds(animTime);

            DoubleAnimation scaleYAnim = new DoubleAnimation();
            Storyboard.SetTargetName(scaleYAnim, "CC_ScaleTransform");
            Storyboard.SetTargetProperty(scaleYAnim, new PropertyPath(ScaleTransform.ScaleYProperty));
            scaleYAnim.From = 3d;
            scaleYAnim.To = 1.0d;
            scaleYAnim.Duration = TimeSpan.FromMilliseconds(animTime);
            expandSB.Children.Add(scaleYAnim);
            expandSB.Children.Add(scaleXAnim);
            expandSB.Begin(cc);
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
