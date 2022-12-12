using cyber_base.implement.utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace cyber_installer.implement.app_support_modules
{
    internal class TaskHandleManager
    {
        public class TaskInfo
        {
            public SemaphoreSlim SemaphoreSlim { get; private set; }
            public string Name { get; set; }
            public TaskInfo(SemaphoreSlim semaphore, string name)
            {
                Name = name;
                SemaphoreSlim = semaphore;
            }
        }
        private Logger _logger = new Logger("TaskHandleManager", "cyber_installer");
        private int _currentTaskCount = 0;

        public int CurrentTaskCount
        {
            get => _currentTaskCount;
            set
            {
                _currentTaskCount = value;
            }
        }

        private Dictionary<string, TaskInfo> _taskSemaphoreMap = new Dictionary<string, TaskInfo>();
        private List<TaskInfo> _handlingTaskQueue = new List<TaskInfo>();

        public bool IsTaskAvailable(string taskTypeKey)
        {
            return _taskSemaphoreMap.ContainsKey(taskTypeKey)
                && _taskSemaphoreMap[taskTypeKey].SemaphoreSlim.CurrentCount > 0;
        }
        public void GenerateNewTaskSemaphore(string taskTypeKey, string taskName, int maxCore, int initCore)
        {
            if (!_taskSemaphoreMap.ContainsKey(taskTypeKey))
            {
                var smp = new SemaphoreSlim(initCore, maxCore);
                var task = new TaskInfo(smp, taskName);
                _taskSemaphoreMap.Add(taskTypeKey, task);
            }
        }

        public async Task ExecuteTask(string taskTypeKey
            , Func<TaskInfo, Task> mainFunc
            , bool bypassIfSemaphoreNotAvaild = false
            , int semaphoreTimeOut = 2000)
        {
            if (_taskSemaphoreMap.ContainsKey(taskTypeKey))
            {
                var smp = _taskSemaphoreMap[taskTypeKey].SemaphoreSlim;
                if (bypassIfSemaphoreNotAvaild)
                {
                    if (smp.CurrentCount == 0)
                    {
                        return;
                    }
                }
                var isSemaphoreAvailable = await smp.WaitAsync(semaphoreTimeOut);
                if (isSemaphoreAvailable)
                {
                    Exception? taskExepction = null;
                    try
                    {
                        _handlingTaskQueue.Insert(0, _taskSemaphoreMap[taskTypeKey]);
                        CurrentTaskCount++;
                        await mainFunc.Invoke(_taskSemaphoreMap[taskTypeKey]);
                        _logger.D("Successfully execute task: " + taskTypeKey);
                    }
                    catch (Exception ex)
                    {
                        taskExepction = ex;
                        _logger.E("Fail to execute task: " + taskTypeKey + "\n" + ex.ToString());
                    }
                    finally
                    {
                        _handlingTaskQueue.Remove(_taskSemaphoreMap[taskTypeKey]);
                        CurrentTaskCount--;
                        smp.Release();
                        if (taskExepction != null)
                        {
                            throw taskExepction;
                        }
                    }
                }
                else
                {
                    _logger.D("Semaphore not available to execute task: " + taskTypeKey);
                }
            }
        }

        public async Task ExecuteTask(string taskTypeKey
           , Action<TaskInfo> mainFunc
           , bool bypassIfSemaphoreNotAvaild = false
           , int semaphoreTimeOut = 2000)
        {
            if (_taskSemaphoreMap.ContainsKey(taskTypeKey))
            {
                var smp = _taskSemaphoreMap[taskTypeKey].SemaphoreSlim;
                if (bypassIfSemaphoreNotAvaild)
                {
                    if (smp.CurrentCount == 0)
                    {
                        return;
                    }
                }
                var isSemaphoreAvailable = await smp.WaitAsync(semaphoreTimeOut);
                if (isSemaphoreAvailable)
                {
                    Exception? taskExepction = null;
                    try
                    {
                        _handlingTaskQueue.Insert(0, _taskSemaphoreMap[taskTypeKey]);
                        CurrentTaskCount++;
                        mainFunc.Invoke(_taskSemaphoreMap[taskTypeKey]);
                        _logger.D("Successfully execute task: " + taskTypeKey);
                    }
                    catch (Exception ex)
                    {
                        taskExepction = ex;
                        _logger.E("Fail to execute task: " + taskTypeKey + "\n" + ex.ToString());
                    }
                    finally
                    {
                        _handlingTaskQueue.Remove(_taskSemaphoreMap[taskTypeKey]);
                        CurrentTaskCount--;
                        smp.Release();
                        if (taskExepction != null)
                        {
                            throw taskExepction;
                        }
                    }
                }
                else
                {
                    _logger.D("Semaphore not available to execute task: " + taskTypeKey);
                }
            }
        }
    }
}
