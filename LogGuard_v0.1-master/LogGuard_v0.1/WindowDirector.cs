using LogGuard_v0._1.Base.AsyncTask;
using LogGuard_v0._1.Utils;
using LogGuard_v0._1.Windows.FloatinWindow;
using LogGuard_v0._1.Windows.MainWindow.View;
using LogGuard_v0._1.Windows.MessageWindow;
using LogGuard_v0._1.Windows.WaitingWindow;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace LogGuard_v0._1
{
    public enum WindowDisplayStatus
    {
        OnMainScreen = 1,
        OnPopupCustomControl = 2,
        AppExit = 10
    }

    public enum OwnerWindow
    {
        Default = 0,
        MainScreen = 1,
    }
    public class WindowDirector
    {
        private MainWindow _mainScreenWindow;
        private FloatingWindow _floatingWindow;
        private WindowDisplayStatus _displayStatus = WindowDisplayStatus.OnMainScreen;

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
        }

        private void MainScreenWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ProcessManagement.GetInstance().KillAllProcess();
        }

        public void ShowMainWindow()
        {
            MainScreenWindow.Show();
            _displayStatus = WindowDisplayStatus.OnMainScreen;
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
            return mesBox.Show();
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


            return mesBox.Show();
        }

        public LogGuardWaitingBoxResult OpenWaitingTaskBox(string content, string title, Func<object, CancellationToken, Task<AsyncTaskResult>> asyncTask, Func<bool> canExecute = null, Action<object, AsyncTaskResult> callback = null, long delayTime = 0)
        {
            var newWaitingBox = new WaitingBox(content, title, asyncTask, canExecute, callback, delayTime, MainScreenWindow);
            return newWaitingBox.Show();
        }

        public void ShowPopupCustomControlWindow(ContentControl cc, UIElement opener, OwnerWindow ownerWindow = OwnerWindow.Default, double width = 500, double height = 400)
        {
            if (_displayStatus == WindowDisplayStatus.OnPopupCustomControl)
            {
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
                    break;
                case OwnerWindow.MainScreen:
                    _floatingWindow = new FloatingWindow(opener, MainScreenWindow, 0, 0);
                    break;
            }
            _floatingWindow.Height = height;
            _floatingWindow.Width = width;

            StartDispandCCAnim(cc, _floatingWindow, 200);

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
        private void StartDispandCCAnim(ContentControl cc, FloatingWindow floatWindow, int animTime)
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
                    _displayStatus = WindowDisplayStatus.OnMainScreen;
                };
                _displayStatus = WindowDisplayStatus.OnPopupCustomControl;
                floatWindow.Show();
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
