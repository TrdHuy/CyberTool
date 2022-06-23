using cyber_base.async_task;
using cyber_base.implement.async_task;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace honeyboard_release_service.implement.ui_event_handler.async_tasks.io_tasks
{
    internal class FindVersionPropertiesFileTask : BaseRTParamAsyncTask
    {
        private readonly string[] VERSION_FILE_NAME = new string[] { "version.properties", "version.gradle" };

        public FindVersionPropertiesFileTask(object param
            , Action<AsyncTaskResult>? completedCallback = null
            , string name = "Searching version properties file!") : base(param, name, completedCallback)
        {
            _estimatedTime = 2000;
            _delayTime = 2000;
            _reportDelay = 100;
        }

        protected override void DoMainTask(object param
            , AsyncTaskResult result
            , CancellationTokenSource token)
        {
            var folderPath = param.ToString();
            if (Directory.Exists(folderPath))
            {
                var files = Directory.GetFiles(folderPath);

                var versionFileName = "";
                foreach (var fileName in VERSION_FILE_NAME)
                {
                    if (files.Any((f) =>
                    {
                        return f.Contains(fileName);
                    }))
                    {
                        versionFileName = fileName;
                        result.Result = folderPath + "\\" + fileName;
                        break;
                    }
                }
            }
        }

        protected override bool IsTaskPossible(object param)
        {
            switch (param)
            {
                case string:
                    return true;
                default:
                    return false;
            }
        }


    }
}
