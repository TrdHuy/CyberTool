using log_guard.definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace log_guard.implement.flow.state_controller
{
    internal class LowCpu_StateController : StateController
    {
        private const int DELAY_RUNNING_THREAD_WHEN_READ_NO_LINE = 10;
        private bool _isNeedGoToSleep;
        private bool _isForceStop = false;

        public bool IsNeedGoToSleep { get => _isNeedGoToSleep; set => _isNeedGoToSleep = value; }

        protected override void OnRunning()
        {
            try
            {
                if (PreviousState == LogGuardState.NONE
                   || PreviousState == LogGuardState.STOP)
                {
                    LwSourceManager.ClearSource();
                }
                var line = "";

                #region Low performance Cpu Run
                /// <summary>
                ///
                /// Luồng chạy này sẽ chiếm ít CPU hơn nhưng tốn thời gian hơn
                ///
                /// </summary>

                while (CurrentState != LogGuardState.STOP
                    && CurrentState != LogGuardState.NONE
                    && !_isForceStop)
                {
                    if (!String.IsNullOrWhiteSpace(line))
                    {
                        LwSourceManager.AddItem(line);
                    }
                    else if (String.IsNullOrEmpty(line))
                    {
                        IsNeedGoToSleep = true;
                    }

                    while (String.IsNullOrEmpty(line = CaptureProc.StandardOutput.ReadLine())
                        || CurrentState == LogGuardState.PAUSING)
                    {
                        lock (SynchronizeStateObject)
                        {
                            // If we've already been told to quit, we don't want to sleep!
                            if (!IsNeedGoToSleep
                            && CurrentState == LogGuardState.RUNNING
                            || _isForceStop)
                            {
                                break;
                            }
                            Monitor.Wait(SynchronizeStateObject
                                , TimeSpan.FromMilliseconds(DELAY_RUNNING_THREAD_WHEN_READ_NO_LINE));

                            if (!IsNeedGoToSleep
                            && CurrentState == LogGuardState.RUNNING
                            || _isForceStop)
                            {
                                break;
                            }
                        }
                    }
                }
                #endregion

            }
            catch (Exception e)
            {
                if (!CaptureProc.HasExited)
                    CaptureProc.Kill();
                CaptureProc.Dispose();
                CaptureProc.Close();
                CaptureProc = null;
            }
            finally
            {
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
            _isForceStop = true;
        }

        protected override void OnStart()
        {
            _isForceStop = false;
        }
    }
}
