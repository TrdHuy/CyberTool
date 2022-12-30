using cyber_base.implement.utils;
using cyber_installer.implement.modules.utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static cyber_installer.definitions.CyberInstallerDefinition;

namespace cyber_installer.implement.app_support_modules
{
    public class TaskHandleManager
    {
        public enum TaskExecuteResult
        {
            Success = 1,
            Fault = 2,
            SemaphoreNotAvailable = 3,
            TaskNotRegistered = 4,
        }

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

        private Dictionary<ManageableTaskKeyDefinition, TaskInfo> _taskSemaphoreMap = new Dictionary<ManageableTaskKeyDefinition, TaskInfo>();
        private List<TaskInfo> _handlingTaskQueue = new List<TaskInfo>();

        public bool IsTaskAvailable(ManageableTaskKeyDefinition taskTypeKey)
        {
            return _taskSemaphoreMap.ContainsKey(taskTypeKey)
                && _taskSemaphoreMap[taskTypeKey].SemaphoreSlim.CurrentCount > 0;
        }

        public void GenerateNewTaskSemaphore(ManageableTaskKeyDefinition taskTypeKey, int maxCore, int initCore)
        {
            if (!_taskSemaphoreMap.ContainsKey(taskTypeKey))
            {
                var smp = new SemaphoreSlim(initCore, maxCore);
                var task = new TaskInfo(smp, taskTypeKey.GetName());
                _taskSemaphoreMap.Add(taskTypeKey, task);
            }
        }

        public async Task<TaskExecuteResult> ExecuteTask(ManageableTaskKeyDefinition taskTypeKey
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
                        return TaskExecuteResult.SemaphoreNotAvailable;
                    }
                }
                var isSemaphoreAvailable = await smp.WaitAsync(semaphoreTimeOut);
                if (isSemaphoreAvailable)
                {
                    var isFault = false;
                    try
                    {
                        _handlingTaskQueue.Insert(0, _taskSemaphoreMap[taskTypeKey]);
                        CurrentTaskCount++;
                        await mainFunc.Invoke(_taskSemaphoreMap[taskTypeKey]);
                        _logger.D("Successfully execute task: " + taskTypeKey);
                    }
                    catch (Exception ex)
                    {
                        _logger.E("Fail to execute task: " + taskTypeKey + "\n" + ex.ToString());
                        isFault = true;
                    }
                    finally
                    {
                        _handlingTaskQueue.Remove(_taskSemaphoreMap[taskTypeKey]);
                        CurrentTaskCount--;
                        smp.Release();
                    }
                    return isFault ? TaskExecuteResult.Fault : TaskExecuteResult.Success;
                }
                else
                {
                    _logger.D("Semaphore not available to execute task: " + taskTypeKey);
                    return TaskExecuteResult.SemaphoreNotAvailable;
                }
            }
            _logger.E("Task was not registered: " + taskTypeKey);
            return TaskExecuteResult.TaskNotRegistered;
        }

        public async Task<TaskExecuteResult> ExecuteTask(ManageableTaskKeyDefinition taskTypeKey
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
                        return TaskExecuteResult.SemaphoreNotAvailable;
                    }
                }
                var isSemaphoreAvailable = await smp.WaitAsync(semaphoreTimeOut);
                if (isSemaphoreAvailable)
                {
                    var isFault = false;
                    try
                    {
                        _handlingTaskQueue.Insert(0, _taskSemaphoreMap[taskTypeKey]);
                        CurrentTaskCount++;
                        mainFunc.Invoke(_taskSemaphoreMap[taskTypeKey]);
                        _logger.D("Successfully execute task: " + taskTypeKey);
                    }
                    catch (Exception ex)
                    {
                        _logger.E("Fail to execute task: " + taskTypeKey + "\n" + ex.ToString());
                        isFault = true;
                    }
                    finally
                    {
                        _handlingTaskQueue.Remove(_taskSemaphoreMap[taskTypeKey]);
                        CurrentTaskCount--;
                        smp.Release();
                    }
                    return isFault ? TaskExecuteResult.Fault : TaskExecuteResult.Success;
                }
                else
                {
                    _logger.D("Semaphore not available to execute task: " + taskTypeKey);
                    return TaskExecuteResult.SemaphoreNotAvailable;
                }
            }
            _logger.E("Task was not registered: " + taskTypeKey);
            return TaskExecuteResult.TaskNotRegistered;
        }
    }
}
