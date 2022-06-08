using cyber_base.async_task;
using cyber_base.implement.utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cyber_base.implement.async_task
{
    public enum MultiAsyncTaskReportType
    {
        EstimatedTime = 0,
        SubTasks = 1,
    }

    public class MultiAsyncTask : BaseAsyncTask
    {
        private static Logger MATLogger = new Logger("MultiAsyncTask");
        private List<BaseAsyncTask> _mainFuncs;
        private CancellationTokenSource _cancellationTokenSource;
        private List<AsyncTaskResult> _results;
        private Func<List<AsyncTaskResult>, AsyncTaskResult, Task<AsyncTaskResult>>? _callback;
        private BaseAsyncTask? _currentExecuteTask;
        private MultiAsyncTaskReportType _rpType;

        protected BaseAsyncTask? CurrentExecuteTask
        {
            get
            {
                return _currentExecuteTask;
            }
            set
            {
                var oldTask = _currentExecuteTask;
                _currentExecuteTask = value;
                if (oldTask != value)
                {
                    CurrentTaskChanged?.Invoke(this, oldTask, value);
                }
            }
        }
        public Func<List<AsyncTaskResult>, AsyncTaskResult, Task<AsyncTaskResult>>? CallbackHandler => _callback;
        public new List<AsyncTaskResult> Result => _results;
        public event CurrentTaskChangedHandler? CurrentTaskChanged;


        public MultiAsyncTask(
            List<BaseAsyncTask> mainFunc
            , CancellationTokenSource cancellationTokenSource
            , Func<List<AsyncTaskResult>, AsyncTaskResult, Task<AsyncTaskResult>>? callback = null
            , string name = ""
            , ulong delayTime = 0
            , int reportDelay = 1000
            , MultiAsyncTaskReportType reportType = MultiAsyncTaskReportType.EstimatedTime)
            : base(name, 0, delayTime, reportDelay)
        {
            _estimatedTime = 0;
            foreach (var ele in mainFunc)
            {
                _estimatedTime += ele.EstimatedTime;
            }
            _mainFuncs = mainFunc;
            _cancellationTokenSource = cancellationTokenSource;
            _results = new List<AsyncTaskResult>();
            _callback = callback;
            _rpType = reportType;
        }

        public int TaskCount => _mainFuncs.Count;

        public override void Cancel()
        {
            if (CurrentExecuteTask != null
                && CurrentExecuteTask.IsCompleted == false
                && CurrentExecuteTask.IsCanceled == false)
            {
                CurrentExecuteTask.Cancel();
            }
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        protected async override Task DoCallback()
        {
            if (CallbackHandler != null)
            {
                await CallbackHandler.Invoke(_results, _result);
            }
        }

        protected async override Task DoDelayForReportTask()
        {
            await Task.Delay(_reportDelay
            , _cancellationTokenSource.Token);
        }

        protected async override Task DoMainFunc()
        {
            int countTaskFinished = 0;
            foreach (var ele in _mainFuncs)
            {
                CurrentExecuteTask = ele;
                await ele.Execute()
                    .ContinueWith((task) =>
                {
                    if (task.IsFaulted)
                    {
                        MATLogger.E(task.Exception?.ToString()
                            ?? ("Task: [" + task.Result.Name + "] was faulted while executing multi async taks"));
                    }
                    else if (task.Result.IsCanceled)
                    {
                        MATLogger.E("Task: [" + task.Result.Name + "] was canceled while executing multi async taks");
                    }
                });

                _results.Add(ele.Result);

                if (_rpType == MultiAsyncTaskReportType.SubTasks)
                {
                    CurrentProgress = Math.Round((double)countTaskFinished
                        / (double)_mainFuncs.Count, 2) * 100;
                }

                if (_cancellationTokenSource.IsCancellationRequested)
                {
                    throw new OperationCanceledException("Task was aborted by user!");
                }
            }
        }

        protected override async Task DoReportTask()
        {
            if (_rpType == MultiAsyncTaskReportType.EstimatedTime)
            {
                await base.DoReportTask();
            }
        }

        protected async override Task DoWaitRestDelay(long rest)
        {
            await Task.Delay(Convert.ToInt32(rest)
                , _cancellationTokenSource.Token);
        }
    }

    public delegate void CurrentTaskChangedHandler(object sender, BaseAsyncTask? oldTask, BaseAsyncTask? newTask);
}
