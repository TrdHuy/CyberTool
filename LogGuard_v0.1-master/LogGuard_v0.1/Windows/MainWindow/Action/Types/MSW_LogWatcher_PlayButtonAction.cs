using LogGuard_v0._1.Base.Utils;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.AndroidLog;
using LogGuard_v0._1.Implement.Device;
using LogGuard_v0._1.Implement.UIEventHandler;
using LogGuard_v0._1.Utils;
using LogGuard_v0._1.Windows.MainWindow.Models;
using LogGuard_v0._1.Windows.MainWindow.ViewModels;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.LogWatcher;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LogGuard_v0._1.Implement.LogGuardFlow;
using LogGuard_v0._1.Base.LogGuardFlow;
using LogGuard_v0._1.Implement.LogGuardFlow.StateController;

namespace LogGuard_v0._1.Windows.MainWindow.Action.Types
{
    public class MSW_LogWatcher_PlayButtonAction : BaseViewModelCommandExecuter
    {
        private static Process _proc;

        private Thread _parsingThread = new Thread(() =>
        {
            try
            {

                _proc.Start();
                ProcessManagement.GetInstance().AddNewProcessID(_proc.Id);

                //if (StateController.Instance.PreState == StateLevel.NONE || StateController.Instance.PreState == StateLevel.STOP)
                //{
                //    _model.LogCount = 0;
                //    _model.LogItemVMs.Clear();
                //}
                string line = "";

                #region Low performance Cpu Run
                /// <summary>
                ///
                /// Luồng chạy này sẽ chiếm ít CPU hơn nhưng tốn thời gian hơn
                ///
                /// </summary>

                //LowCpu_StateControllerImpl.Current.Start();
                //while (true)
                //{
                //    if (!String.IsNullOrWhiteSpace(line))
                //    {
                //        LogInfo lif = LogInfoManagerImpl.Current.ParseLogInfos(line, false, false);
                //        if (lif != null)
                //        {
                //            LogWatcherItemViewModel livm = new LogWatcherItemViewModel(lif);
                //            _modelStatic.LogItemVMs.Add(livm);
                //        }
                //    }
                //    else if (String.IsNullOrEmpty(line))
                //    {
                //        StateControllerImpl.Current.IsNeedToGoSleep = true;
                //    }

                //    while (String.IsNullOrEmpty(line = _proc.StandardOutput.ReadLine()))
                //    {
                //        lock (StateControllerImpl.Current.SynchronizeStateObject)
                //        {
                //            // If we've already been told to quit, we don't want to sleep!
                //            if (!StateControllerImpl.Current.IsNeedToGoSleep
                //            && StateControllerImpl.Current.CurrentState == LogGuardState.RUNNING)
                //            {
                //                break;
                //            }
                //            Monitor.Wait(StateControllerImpl.Current.SynchronizeStateObject
                //                , TimeSpan.FromMilliseconds(StateControllerImpl.DELAY_RUNNING_THREAD_WHEN_READ_NO_LINE));

                //            if (!StateControllerImpl.Current.IsNeedToGoSleep
                //            && StateControllerImpl.Current.CurrentState == LogGuardState.RUNNING)
                //            {
                //                break;
                //            }
                //        }

                //    }

                //}
                #endregion

                #region High performance Cpu Run
                /// <summary>
                ///
                /// _Luồng chạy này sẽ chiếm nhiều CPU hơn, nhưng chỉ lúc đầu 
                /// do lượng cache log trong device là lớn. Thời gian để quét và parse rất nhanh
                /// _Tận dụng được tối đa hiệu năng của CPU
                /// => Ưu tiên dùng luồng chạy này
                /// </summary>

                while ((line = _proc.StandardOutput.ReadLine()) != null)
                {
                    lock (HighCpu_StateController.Current.SynchronizeStateObject)
                    {
                        LogInfo lif = LogInfoManagerImpl.Current.ParseLogInfos(line, false, false);
                        if (lif != null)
                        {
                            LogWatcherItemViewModel livm = new LogWatcherItemViewModel(lif);
                            _modelStatic.LogItemVMs.Add(livm);
                        }
                    }

                    HighCpu_StateController.Current.PausingEvent.WaitOne();

                    if (HighCpu_StateController.Current.StopEvent.WaitOne(0))
                        break;
                }
                #endregion

                if (!_proc.HasExited)
                    _proc.Kill();
            }
            catch (Exception e)
            {
            }
            finally
            {
                //StopAllActivities();
                _proc.Dispose();
                _proc.Close();
            }
        });


        protected MainWindowViewModel MSWViewModel
        {
            get
            {
                return ViewModel as MainWindowViewModel;
            }
        }

        private static MainWindowViewModel _modelStatic;
        public MSW_LogWatcher_PlayButtonAction(string actionID, string builderID, BaseViewModel viewModel, ILogger logger) : base(actionID, builderID, viewModel, logger) { }

        protected override void ExecuteCommand()
        {
            base.ExecuteCommand();

            if (HighCpu_StateController.Current.CurrentState == LogGuardState.NONE
                || HighCpu_StateController.Current.CurrentState == LogGuardState.STOP)
            {
                MSWViewModel.CurrentLogGuardState = LogGuardState.RUNNING;
                HighCpu_StateController.Current.Start();
            }
            else if (HighCpu_StateController.Current.CurrentState == LogGuardState.RUNNING)
            {
                MSWViewModel.CurrentLogGuardState = LogGuardState.PAUSING;
                HighCpu_StateController.Current.Pause();
            }
            else if (HighCpu_StateController.Current.CurrentState == LogGuardState.PAUSING)
            {
                MSWViewModel.CurrentLogGuardState = LogGuardState.RUNNING;

                HighCpu_StateController.Current.Resume();
            }


            //if (LowCpu_StateController.Current.CurrentState == LogGuardState.NONE
            //    || LowCpu_StateController.Current.CurrentState == LogGuardState.STOP)
            //{
            //    MSWViewModel.CurrentLogGuardState = LogGuardState.RUNNING;
            //    LowCpu_StateController.Current.Start();
            //}
            //else if (LowCpu_StateController.Current.CurrentState == LogGuardState.RUNNING)
            //{
            //    MSWViewModel.CurrentLogGuardState = LogGuardState.PAUSING;
            //    LowCpu_StateController.Current.Pause();
            //}
            //else if (LowCpu_StateController.Current.CurrentState == LogGuardState.PAUSING)
            //{
            //    MSWViewModel.CurrentLogGuardState = LogGuardState.RUNNING;

            //    LowCpu_StateController.Current.Resume();
            //}

        }


    }
}
