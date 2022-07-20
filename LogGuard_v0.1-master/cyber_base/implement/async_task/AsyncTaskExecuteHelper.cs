using cyber_base.async_task;
using cyber_base.implement.utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cyber_base.implement.async_task
{
    /// <summary>
    /// Class này hỗ trợ xử lý nhiều async task cùng lúc.
    /// Mỗi task sẽ được xử lý trên 1 core, có thể khai báo
    /// số core tối đa bằng thuộc tính maximumCore.
    /// 
    /// Ngoài ra, khi số lượng task đang xử lý đạt tối đa
    /// các task được thêm vào pool để xử lý sẽ phải chờ trong 1 auto
    /// resize stack, stack này cũng sẽ có số lượng slot tối đa
    /// cho các task đang chờ để xử lý, các task vào sau sẽ
    /// đẩy các task cũ nhất khỏi stack.
    /// </summary>
    public class AsyncTaskExecuteHelper
    {
        private ObservableCollection<BaseAsyncTask> _taskPool;
        private SemaphoreSlim _semaphore;
        private CancellationTokenSource _tokenSource;
        private AutoResizeStack<BaseAsyncTask> _waitingPool;
        public AsyncTaskExecuteHelper(int maximumCore = 10
            , int waitingCapacity = 10)
        {
            _taskPool = new ObservableCollection<BaseAsyncTask>();
            _semaphore = new SemaphoreSlim(maximumCore, maximumCore);
            _tokenSource = new CancellationTokenSource();
            _waitingPool = new AutoResizeStack<BaseAsyncTask>(waitingCapacity);
        }

        public async void AddTask(BaseAsyncTask newTask)
        {
            if (!newTask.IsCompleted
                && !newTask.IsCanceled
                && !newTask.IsFaulted)
            {
                if (!newTask.IsExecuting)
                {
                    _waitingPool.Push(newTask);
                    await _semaphore.WaitAsync(_tokenSource.Token);
                    var executeTask = _waitingPool.Pop();

                    if (_tokenSource.IsCancellationRequested)
                    {
                        _semaphore.Release();
                        return;
                    }
                    try
                    {
                        if (executeTask != null)
                        {
                            _taskPool.Add(executeTask);
                            await executeTask.Execute();
                            _taskPool.Remove(executeTask);
                        }
                    }
                    catch
                    {
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                }
                else
                {
                    throw new InvalidOperationException("Please do not add task which being executed!");
                }
            }
        }

        public async void ForceAddTask(BaseAsyncTask newTask)
        {
            if (!newTask.IsCompleted
                && !newTask.IsCanceled
                && !newTask.IsFaulted)
            {
                if (!newTask.IsExecuting)
                {

                    if (_semaphore.CurrentCount == 0)
                    {
                        BaseAsyncTask? abortTask = null;
                        foreach (var task in _taskPool)
                        {
                            if (!task.IsCompleted
                                && !task.IsCanceled
                                && !task.IsFaulted
                                 && !task.IsCompletedCallback)
                            {
                                task.Cancel();
                                abortTask = task;
                                break;
                            }
                        }
                        if (abortTask != null)
                        {
                            _taskPool.Remove(abortTask);
                        }
                    }

                    await _semaphore.WaitAsync(_tokenSource.Token);

                    try
                    {
                        if (newTask != null)
                        {
                            _taskPool.Add(newTask);
                            await newTask.Execute();
                            _taskPool.Remove(newTask);
                        }
                    }
                    catch
                    {
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                }
                else
                {
                    throw new InvalidOperationException("Please do not add task which being executed!");
                }
            }
        }
        
        public void Cancel()
        {
            _tokenSource.Cancel();
            foreach (var task in _taskPool)
            {
                if (task.IsExecuting)
                {
                    task.Cancel();
                }
            }
        }

        public void Refresh()
        {
            _tokenSource.Cancel();
            _taskPool.Clear();
            _tokenSource.Dispose();
            _tokenSource = new CancellationTokenSource();
        }
    }
}
