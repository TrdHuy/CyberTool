using log_guard.definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace log_guard.implement.flow.state_controller
{
    internal class HighCpu_StateController : StateController
    {

        public ManualResetEvent PausingEvent = new ManualResetEvent(true);
        public ManualResetEvent StopEvent = new ManualResetEvent(false);

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

                #region High performance Cpu Run
                /// <summary>
                /// 
                /// _Luồng chạy này sẽ chiếm nhiều CPU hơn, nhưng chỉ lúc đầu 
                /// do lượng cache log trong device là lớn. 
                /// _Thời gian để quét và parse rất nhanh.
                /// _Tận dụng được tối đa hiệu năng của CPU
                /// => Ưu tiên dùng luồng chạy này
                /// 
                /// </summary>

                while ((line = CaptureProc.StandardOutput.ReadLine()) != null)
                {
                    lock (SynchronizeStateObject)
                    {
                        LwSourceManager.AddItem(line);
                    }

                    PausingEvent.WaitOne();

                    if (StopEvent.WaitOne(0))
                        break;
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
            PausingEvent.Reset();
        }

        protected override void OnResume()
        {
            PausingEvent.Set();
        }

        protected override void OnStop()
        {
            StopEvent.Set();
            PausingEvent.Set();
        }

        protected override void OnStart()
        {
            StopEvent.Reset();
        }
    }
}
