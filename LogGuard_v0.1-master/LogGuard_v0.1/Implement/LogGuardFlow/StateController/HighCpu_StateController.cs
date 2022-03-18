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
    public class HighCpu_StateController : StateControllerImpl
    {
        private static HighCpu_StateController _instance;

        public ManualResetEvent PausingEvent = new ManualResetEvent(true);
        public ManualResetEvent StopEvent = new ManualResetEvent(false);

       
        private HighCpu_StateController() : base()
        {
        }

        protected override void OnRunning()
        {

            try
            {
                
                if (PreviousState == LogGuardState.NONE 
                    || PreviousState == LogGuardState.STOP)
                {
                    LGSourceManager.ClearSource();
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
                        LGSourceManager.AddItem(line);
                    }

                    PausingEvent.WaitOne();

                    if (StopEvent.WaitOne(0))
                        break;
                }
                #endregion
            }
            catch (Exception e)
            {
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

        public static HighCpu_StateController Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new HighCpu_StateController();
                }
                return _instance;
            }
        }

    }
}
