using LogGuard_v0._1.Base.LogGuardFlow;
using LogGuard_v0._1.Implement.AndroidLog;
using LogGuard_v0._1.Implement.Device;
using LogGuard_v0._1.Utils;
using LogGuard_v0._1.Windows.MainWindow.Models;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.LogWatcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Implement.LogGuardFlow.StateController
{
    public class LowCpu_StateController : StateControllerImpl
    {
        private const int DELAY_RUNNING_THREAD_WHEN_READ_NO_LINE = 100;
        private static LowCpu_StateController _instance;
        private bool _isNeedGoToSleep;
        public bool IsNeedGoToSleep { get => _isNeedGoToSleep; set => _isNeedGoToSleep = value; }
        private LowCpu_StateController() : base()
        {
        }

        protected override void OnRunning()
        {
            var proc = DeviceCmdExecuterImpl.Current.CreateProcess(" logcat");

            try
            {
                proc.Start();
                ProcessManagement.GetInstance().AddNewProcessID(proc.Id);

                if (PreviousState == LogGuardState.NONE
                   || PreviousState == LogGuardState.STOP)
                {
                    LGSourceManager.ClearSource();
                }
                var line = "";

                #region Low performance Cpu Run
                /// <summary>
                ///
                /// Luồng chạy này sẽ chiếm ít CPU hơn nhưng tốn thời gian hơn
                ///
                /// </summary>

                while (CurrentState != LogGuardState.STOP
                    && CurrentState != LogGuardState.NONE)
                {
                    if (!String.IsNullOrWhiteSpace(line))
                    {
                        LogInfo lif = LogInfoManagerImpl.Current.ParseLogInfos(line, false, false);
                        if (lif != null)
                        {
                            LogWatcherItemViewModel livm = new LogWatcherItemViewModel(lif);
                            LGSourceManager.AddItem(livm);
                        }
                    }
                    else if (String.IsNullOrEmpty(line))
                    {
                        IsNeedGoToSleep = true;
                    }

                    while (String.IsNullOrEmpty(line = proc.StandardOutput.ReadLine())
                        || CurrentState == LogGuardState.PAUSING)
                    {
                        lock (SynchronizeStateObject)
                        {
                            // If we've already been told to quit, we don't want to sleep!
                            if (!IsNeedGoToSleep
                            && CurrentState == LogGuardState.RUNNING)
                            {
                                break;
                            }
                            Monitor.Wait(SynchronizeStateObject
                                , TimeSpan.FromMilliseconds(DELAY_RUNNING_THREAD_WHEN_READ_NO_LINE));

                            if (!IsNeedGoToSleep
                            && CurrentState == LogGuardState.RUNNING)
                            {
                                break;
                            }
                        }
                    }
                }
                #endregion

                if (!proc.HasExited)
                    proc.Kill();
            }
            catch (Exception e)
            {
            }
            finally
            {
                //StopAllActivities();
                proc.Dispose();
                proc.Close();
            }
        }

        protected override void OnPause()
        {
        }

        protected override void OnResume()
        {
        }

        protected override void OnStop()
        {
        }

        protected override void OnStart()
        {
            RunningThread.Start();
        }

        public static LowCpu_StateController Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LowCpu_StateController();
                }
                return _instance;
            }
        }
    }
}
