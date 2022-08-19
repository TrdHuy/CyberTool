using LogGuard_v0._1.Base.AsyncTask;
using LogGuard_v0._1.Utils;
using LogGuard_v0._1.Windows.FloatinWindow;
using LogGuard_v0._1.Windows.MainWindow.View;
using LogGuard_v0._1.Windows.MessageWindow;
using LogGuard_v0._1.Windows.WaitingWindow;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace LogGuard_v0._1
{
    public enum OwnerWindow
    {
        Default = 0,
        MainScreen = 1,
    }
    public class WindowDirector
    {
        private Dictionary<ContentControl, FloatingWindow> _floatingWindowMap;
        private MainWindow _mainScreenWindow;
        private FloatingWindow _floatingWindow;

        public MainWindow MainScreenWindow
        {
            get
            {
                if (_mainScreenWindow == null)
                {
                    _mainScreenWindow = new MainWindow();
                }
                return _mainScreenWindow;
            }
            set
            {
                _mainScreenWindow = value;
            }
        }

        public WindowDirector()
        {
            MainScreenWindow.Closing += MainScreenWindow_Closing;
            _floatingWindowMap = new Dictionary<ContentControl, FloatingWindow>();
        }

        private void MainScreenWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ProcessManagement.GetInstance().KillAllProcess();
        }

        public void ShowMainWindow()
        {
            MainScreenWindow.Show();
        }

        public LogGuardMesBoxResult ShowErrorBox(string error)
        {
            Windows.MessageWindow.MessageBox mesBox = new Windows.MessageWindow.MessageBox(
                "Error",
                Application.Current.Resources["QuestionPathGeomerty"] as string,
                error,
                "",
                "",
                "Continue",
                "",
                MainScreenWindow
                );
            return mesBox.ShowDialog();
        }

        public LogGuardMesBoxResult ShowWarningBox(string warning, bool isDialog)
        {
            Windows.MessageWindow.MessageBox mesBox = new Windows.MessageWindow.MessageBox(
                "Warning",
                Application.Current.Resources["QuestionPathGeomerty"] as string,
                warning,
                "",
                "",
                "Continue",
                "",
                MainScreenWindow
                );
            LogGuardMesBoxResult res = LogGuardMesBoxResult.Continue;
            if (isDialog)
            {
                res = mesBox.ShowDialog();
            }
            else
            {
                mesBox.Show();
            }
            return res;
        }
        public LogGuardMesBoxResult ShowEscapeCaptureLogWarningBox()
        {
            Windows.MessageWindow.MessageBox mesBox = new Windows.MessageWindow.MessageBox(
                "Warning",
                Application.Current.Resources["QuestionPathGeomerty"] as string,
                "Do you want to save the captured logs?",
                "Yes",
                "No",
                "Continue without saving",
                "",
                MainScreenWindow
                );


            return mesBox.ShowDialog();
        }

        public LogGuardWaitingBoxResult OpenWaitingTaskBox(string content, string title, Func<object, CancellationToken, Task<AsyncTaskResult>> asyncTask, Func<bool> canExecute = null, Action<object, AsyncTaskResult> callback = null, long delayTime = 0)
        {
            var newWaitingBox = new WaitingBox(content, title, asyncTask, canExecute, callback, delayTime, MainScreenWindow);
            return newWaitingBox.Show();
        }

        public void ShowPopupCustomControlWindow(ContentControl cc
            , UIElement opener
            , OwnerWindow ownerWindow = OwnerWindow.Default
            , double width = 500
            , double height = 400
            , object dataContext = null
            , Action<object> windowShowedCallback = null
            , string title = "Floating window")
        {
            if (_floatingWindowMap.ContainsKey(cc))
            {
                _floatingWindow = _floatingWindowMap[cc];
                if (_floatingWindow != null && _floatingWindow.WindowState == WindowState.Minimized)
                {
                    _floatingWindow.WindowState = WindowState.Normal;
                }
                return;
            }

            switch (ownerWindow)
            {
                case OwnerWindow.Default:
                    _floatingWindow = new FloatingWindow(opener, null, width, height);
                    _floatingWindow.DataContext = dataContext;
                    _floatingWindow.Title = title;
                    break;
                case OwnerWindow.MainScreen:
                    _floatingWindow = new FloatingWindow(opener, MainScreenWindow, 0, 0);
                    _floatingWindow.DataContext = dataContext;
                    _floatingWindow.Title = title;
                    break;
            }
            _floatingWindow.Height = height;
            _floatingWindow.Width = width;

            _floatingWindowMap.Add(cc, _floatingWindow);

            StartDispandCCAnim(cc, _floatingWindow, 200, windowShowedCallback);

        }

        public string OpenSaveLogFileDialogWindow()
        {
            System.Windows.Forms.SaveFileDialog saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog1.Filter = "Text files (*.txt)|*.txt|Log files (*.log)|*.log";
            saveFileDialog1.Title = "Save an log File";
            saveFileDialog1.FileName = "Log_" + DateTime.Now.ToString("MM-dd-yyyy_HH-mm-ss");

            saveFileDialog1.ShowDialog();
            return saveFileDialog1.FileName;
        }

        public string OpenFileChooserDialogWindow(string title = "Choose a log file", string filter = "Text files (*.txt)|*.txt|Log files (*.log)|*.log")
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = filter;
            openFileDialog.Title = title;
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                return openFileDialog.FileName;
            return "";
        }

        private void StartDispandCCAnim(ContentControl cc
            , FloatingWindow floatWindow
            , int animTime
            , Action<object> windowShowedCallback = null)
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
                    _floatingWindowMap.Remove(cc);
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


    }


}
