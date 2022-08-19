using cyber_base.async_task;
using cyber_base.implement.async_task;
using progtroll.implement.module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progtroll.implement.async_task_execute_helper
{
    internal class AsyncTaskManager : BasePublisherModule
    {
        private AsyncTaskExecuteHelper _versionHistoryItemsLoadingTaskHelper;

        public static AsyncTaskManager? Current
        {
            get
            {
                return PublisherModuleManager.ATM_Instance;
            }
        }

        private AsyncTaskManager()
        {
            _versionHistoryItemsLoadingTaskHelper = new AsyncTaskExecuteHelper(maximumCore: 4
                , waitingCapacity: 4);
        }

        public void AddVersionPropertiesLoadingTask(BaseAsyncTask task)
        {
            if (task != null)
            {
                _versionHistoryItemsLoadingTaskHelper.AddTask(task);
            }
        }

        public void ForceAddVersionPropertiesLoadingTask(BaseAsyncTask task)
        {
            if (task != null)
            {
                _versionHistoryItemsLoadingTaskHelper.ForceAddTask(task);
            }
        }
    }
}
