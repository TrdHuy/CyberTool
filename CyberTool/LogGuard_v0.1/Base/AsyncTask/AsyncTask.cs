using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Base.AsyncTask
{
    public class AsyncTask : IAsyncTask
    {
        private long _delayTime;
        private AsyncTaskResult _result;
        private bool _isCompleted;
        private bool _isCompletedCallback;
        private bool _isCanceled;

        private Action<object, AsyncTaskResult> _paramExecuteCallback;
        private Action<AsyncTaskResult> _callback;
        private Func<Task<AsyncTaskResult>> _execute;
        private Func<CancellationToken, Task<AsyncTaskResult>> _cancelableExecute;
        private Func<object, CancellationToken, Task<AsyncTaskResult>> _paramExecute;
        private Func<bool> _canExecute;
        private CancellationTokenSource _cancellationTokenSource;

        public long DelayTime { get => _delayTime; }
        public AsyncTaskResult Result { get => _result; }

        public Func<bool> CanExecute => _canExecute;

        public Func<Task<AsyncTaskResult>> Execute => _execute;

        public Func<CancellationToken, Task<AsyncTaskResult>> CancelableExecute => _cancelableExecute;

        public Func<object, CancellationToken, Task<AsyncTaskResult>> ParamExecute => _paramExecute;

        public Action<AsyncTaskResult> CallbackHandler => _callback;
        public Action<object, AsyncTaskResult> ParamExecuteCallbackHandler => _paramExecuteCallback;

        public bool IsCompletedCallback { get => _isCompletedCallback; private set => _isCompletedCallback = value; }

        public bool IsCompleted
        {
            get => _isCompleted;
            private set
            {
                var oldVal = _isCompleted;
                _isCompleted = value;

                if (oldVal != value)
                {
                    OnCompletedChanged?.Invoke(this, oldVal, value);
                }
            }
        }

        public bool IsCanceled
        {
            get => _isCanceled;
            private set
            {
                var oldVal = _isCanceled;
                _isCanceled = value;

                if (oldVal != value)
                {
                    OncanceldChanged?.Invoke(this, oldVal, value);
                }
            }
        }


        public AsyncTask(Func<Task<AsyncTaskResult>> execute)
        {
            InitializeAsyncTask(execute);
        }

        public AsyncTask(Func<Task<AsyncTaskResult>> execute, Func<bool> canExecute)
        {
            InitializeAsyncTask(execute, canExecute);
        }

        public AsyncTask(Func<Task<AsyncTaskResult>> execute, Func<bool> canExecute, Action<AsyncTaskResult> callback)
        {
            InitializeAsyncTask(execute, canExecute, callback);
        }

        public AsyncTask(Func<Task<AsyncTaskResult>> execute, Func<bool> canExecute, Action<AsyncTaskResult> callback, long delayTime)
        {
            InitializeAsyncTask(execute, canExecute, callback, delayTime);
        }

        public AsyncTask(Func<CancellationToken, Task<AsyncTaskResult>> cancelablExecute, Func<bool> canExecute, Action<AsyncTaskResult> callback, long delayTime, CancellationTokenSource cancellationTokenSource)
        {
            InitializeAsyncTask(cancelablExecute, canExecute, callback, delayTime, cancellationTokenSource);
        }

        public AsyncTask(Func<object, CancellationToken, Task<AsyncTaskResult>> paramExecute, Func<bool> canExecute, Action<object, AsyncTaskResult> callback, long delayTime, CancellationTokenSource cancellationTokenSource)
        {
            InitializeAsyncTask(paramExecute, canExecute, callback, delayTime, cancellationTokenSource);
        }

        private void InitializeAsyncTask(
            Func<Task<AsyncTaskResult>> execute,
            Func<bool> canExecute = null,
            Action<AsyncTaskResult> callback = null,
            long delayTime = 0)
        {
            _execute = execute;
            _canExecute = canExecute;
            _callback = callback;
            _delayTime = delayTime;
            _result = new AsyncTaskResult(null, MessageAsyncTaskResult.Non);
        }

        private void InitializeAsyncTask(
            Func<CancellationToken, Task<AsyncTaskResult>> cancelablExecute,
            Func<bool> canExecute = null,
            Action<AsyncTaskResult> callback = null,
            long delayTime = 0,
            CancellationTokenSource cancellationTokenSource = null)
        {
            _cancelableExecute = cancelablExecute;
            _canExecute = canExecute;
            _callback = callback;
            _delayTime = delayTime;
            _result = new AsyncTaskResult(null, MessageAsyncTaskResult.Non);
            _cancellationTokenSource = cancellationTokenSource;
        }

        private void InitializeAsyncTask(
            Func<object, CancellationToken, Task<AsyncTaskResult>> paramExecute,
            Func<bool> canExecute = null,
            Action<object, AsyncTaskResult> callback = null,
            long delayTime = 0,
            CancellationTokenSource cancellationTokenSource = null)
        {
            _paramExecute = paramExecute;
            _canExecute = canExecute;
            _paramExecuteCallback = callback;
            _delayTime = delayTime;
            _result = new AsyncTaskResult(null, MessageAsyncTaskResult.Non);
            _cancellationTokenSource = cancellationTokenSource;
        }

        public event IsCompletedChangedHandler OnCompletedChanged;
        public event IscanceldChangedHandler OncanceldChanged;

        public static async void AsyncExecute(AsyncTask asyncTask, CancellationToken token)
        {
            if (asyncTask == null)
                return;

            var asyncTaskResult = new AsyncTaskResult(null, MessageAsyncTaskResult.Non);

            try
            {
                var asynTaskExecuteWatcher = Stopwatch.StartNew();

                var canExecute = asyncTask.CanExecute == null ? true : (bool)asyncTask.CanExecute?.Invoke();

                if (canExecute)
                {
                    try
                    {
                        asyncTaskResult = await Task.Run<AsyncTaskResult>(asyncTask.Execute, token);

                        if (token.IsCancellationRequested)
                        {
                            token.ThrowIfCancellationRequested();
                        }

                        if (asyncTaskResult != null && asyncTaskResult.MesResult == MessageAsyncTaskResult.Aborted)
                        {
                            throw new OperationCanceledException(asyncTaskResult.Messsage);
                        }
                    }
                    catch
                    {
                        asyncTask.IsCanceled = true;
                    }

                    asynTaskExecuteWatcher.Stop();
                    long restLoadingTime = asyncTask.DelayTime - asynTaskExecuteWatcher.ElapsedMilliseconds;
                    if (restLoadingTime > 0 && !asyncTask.IsCanceled)
                    {
                        try
                        {
                            await Task.Delay(Convert.ToInt32(restLoadingTime), token);
                        }
                        catch
                        {
                            asyncTask.IsCanceled = true;
                        }
                    }
                    asyncTask.IsCompleted = true;

                    asyncTask.CallbackHandler?.Invoke(asyncTaskResult);
                    asyncTask.IsCompletedCallback = true;
                }

            }
            catch (OperationCanceledException)
            {
                asyncTask.IsCompletedCallback = false;
                asyncTask.IsCompleted = false;
                asyncTask.IsCanceled = true;
            }
        }

        // TODO: Cần viết lại method này ngoài UI thread
        // Có thể sử dụng Task.Run để đạt được
        public static async void AsyncExecute(AsyncTask asyncTask)
        {
            if (asyncTask == null)
                return;

            var asyncTaskResult = new AsyncTaskResult(null, MessageAsyncTaskResult.Non);

            try
            {
                var asynTaskExecuteWatcher = Stopwatch.StartNew();

                var canExecute = asyncTask.CanExecute == null ? true : (bool)asyncTask.CanExecute?.Invoke();

                if (canExecute)
                {
                    try
                    {
                        asyncTaskResult = await Task.Run<AsyncTaskResult>(asyncTask.Execute);

                        if (asyncTaskResult != null && asyncTaskResult.MesResult == MessageAsyncTaskResult.Aborted)
                        {
                            throw new OperationCanceledException(asyncTaskResult.Messsage);
                        }
                    }
                    catch
                    {
                        asyncTask.IsCanceled = true;
                    }

                    asynTaskExecuteWatcher.Stop();
                    long restLoadingTime = asyncTask.DelayTime - asynTaskExecuteWatcher.ElapsedMilliseconds;
                    if (restLoadingTime > 0 && !asyncTask.IsCanceled)
                    {
                        await Task.Delay(Convert.ToInt32(restLoadingTime));
                    }
                    asyncTask.IsCompleted = true;

                    asyncTask.CallbackHandler?.Invoke(asyncTaskResult);
                    asyncTask.IsCompletedCallback = true;
                }

            }
            catch (OperationCanceledException)
            {
                asyncTask.IsCompletedCallback = false;
                asyncTask.IsCompleted = false;
                asyncTask.IsCanceled = true;
            }
        }

        // TODO: Cần viết lại method này ngoài UI thread
        // Có thể sử dụng Task.Run để đạt được
        public static async void CancelableAsyncExecute(AsyncTask asyncTask)
        {
            if (asyncTask == null)
                return;

            var asyncTaskResult = new AsyncTaskResult(null, MessageAsyncTaskResult.Non);

            try
            {
                var asynTaskExecuteWatcher = Stopwatch.StartNew();

                var canExecute = asyncTask.CanExecute == null ? true : (bool)asyncTask.CanExecute?.Invoke();

                if (canExecute)
                {
                    try
                    {
                        asyncTaskResult = await asyncTask.CancelableExecute?.Invoke(asyncTask._cancellationTokenSource.Token);
                        if (asyncTask._cancellationTokenSource.Token.IsCancellationRequested)
                        {
                            throw new OperationCanceledException();
                        }

                        if (asyncTaskResult != null && asyncTaskResult.MesResult == MessageAsyncTaskResult.Aborted)
                        {
                            throw new OperationCanceledException(asyncTaskResult.Messsage);
                        }
                    }
                    catch (Exception ex)
                    {
                        asyncTask.IsCanceled = true;
                    }

                    asynTaskExecuteWatcher.Stop();

                    //
                    //Console.WriteLine("Execute time = " + asynTaskExecuteWatcher.ElapsedMilliseconds + "(ms)");

                    long restLoadingTime = asyncTask.DelayTime - asynTaskExecuteWatcher.ElapsedMilliseconds;
                    if (restLoadingTime > 0 && !asyncTask.IsCanceled)
                    {
                        try
                        {
                            await Task.Delay(Convert.ToInt32(restLoadingTime), asyncTask._cancellationTokenSource.Token);
                        }
                        catch
                        {
                            asyncTask.IsCanceled = true;
                        }
                    }
                    asyncTask.IsCompleted = true;

                    //asynTaskExecuteWatcher = Stopwatch.StartNew();
                    asyncTask.CallbackHandler?.Invoke(asyncTaskResult);
                    //asynTaskExecuteWatcher.Stop();
                    //Console.WriteLine("Callback time = " + asynTaskExecuteWatcher.ElapsedMilliseconds + "(ms)");

                    asyncTask.IsCompletedCallback = true;
                }

            }
            catch (OperationCanceledException)
            {
                asyncTask.IsCompletedCallback = false;
                asyncTask.IsCompleted = false;
                asyncTask.IsCanceled = true;
            }
        }


        #region ParamAsyncExecute
        public static async void ParamAsyncExecute(AsyncTask asyncTask, object param, bool isAsyncCallback = false)
        {

            var asyncTaskResult = new AsyncTaskResult(null, MessageAsyncTaskResult.Non);

            try
            {
                var asynTaskExecuteWatcher = Stopwatch.StartNew();
                var canExecute = asyncTask.CanExecute == null ? true : (bool)asyncTask.CanExecute?.Invoke();

                if (canExecute)
                {
                    try
                    {
                        ///====================
                        /// Execute task
                        ///====================
                        asyncTaskResult = await Task.Run(async () =>
                        {
                            var res = await asyncTask.ParamExecute?.Invoke(param, asyncTask._cancellationTokenSource.Token);
                            return res;
                        }, asyncTask._cancellationTokenSource.Token);

                        if (asyncTask._cancellationTokenSource.Token.IsCancellationRequested)
                        {
                            throw new OperationCanceledException();
                        }

                        if (asyncTaskResult != null && asyncTaskResult.MesResult == MessageAsyncTaskResult.Aborted)
                        {
                            throw new OperationCanceledException(asyncTaskResult.Messsage);
                        }
                    }
                    catch (Exception ex)
                    {
                        asyncTask.IsCanceled = true;
                        asyncTaskResult.MesResult = MessageAsyncTaskResult.Aborted;
                    }

                    asynTaskExecuteWatcher.Stop();

                    //
                    //Console.WriteLine("Execute time = " + asynTaskExecuteWatcher.ElapsedMilliseconds + "(ms)");

                    long restLoadingTime = asyncTask.DelayTime - asynTaskExecuteWatcher.ElapsedMilliseconds;
                    if (restLoadingTime > 0 && !asyncTask.IsCanceled)
                    {
                        try
                        {
                            await Task.Delay(Convert.ToInt32(restLoadingTime), asyncTask._cancellationTokenSource.Token);
                        }
                        catch
                        {
                            asyncTask.IsCanceled = true;
                        }
                    }
                    asyncTask.IsCompleted = true;

                    ///====================
                    /// Callback method
                    ///====================
                    if (isAsyncCallback)
                    {
                        await Task.Run(() =>
                        {
                            asyncTask.ParamExecuteCallbackHandler?.Invoke(param, asyncTaskResult);
                        });
                    }
                    else
                    {
                        asyncTask.ParamExecuteCallbackHandler?.Invoke(param, asyncTaskResult);
                    }

                    asyncTask.IsCompletedCallback = true;
                }

            }
            catch (OperationCanceledException)
            {
                asyncTask.IsCompletedCallback = false;
                asyncTask.IsCompleted = false;
                asyncTask.IsCanceled = true;
            }
        }

        #endregion

        public static void CancelAsyncExecute(AsyncTask asyncTask)
        {
            asyncTask._cancellationTokenSource?.Cancel();
        }

    }
}
