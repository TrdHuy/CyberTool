using cyber_base.async_task;
using progtroll.implement.log_manager;
using progtroll.implement.project_manager;
using progtroll.models.VOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace progtroll.implement.ui_event_handler.async_tasks.io_tasks
{
    internal class ModifyVersionPropertiesFileTask : BaseRTParamAsyncTask
    {
        private string _folderPath = "";
        private string _versionFileName = "";
        private VersionPropertiesVO? _modifiedVersion;

        private Dictionary<string, string> _propertiesMap = new Dictionary<string, string>();

        public ModifyVersionPropertiesFileTask(object param
             , Action<AsyncTaskResult>? completedCallback = null
             , string name = "Modifying version properties file")
             : base(param, name, completedCallback)
        {
            switch (param)
            {
                case object[] data:
                    if (data.Length == 3)
                    {
                        _folderPath = data[0].ToString() ?? "";
                        _versionFileName = data[1].ToString() ?? "";
                        _modifiedVersion = data[2] as VersionPropertiesVO;
                    }
                    else
                    {
                        throw new InvalidDataException("Param must be an array of object has 3 elements");
                    }
                    break;
                default:
                    throw new InvalidDataException("Param must be an array of object has 3 elements");
            }

            _estimatedTime = 2000;
            _reportDelay = 100;
            _delayTime = 2000;
        }


        protected override void DoCallback(object param, AsyncTaskResult result)
        {
            result.Result = _propertiesMap;
        }

        protected override void DoMainTask(object param, AsyncTaskResult result, CancellationTokenSource token)
        {
            LogManager.Current.AppendLogLine("Modifying version properties file", true);

            if (_modifiedVersion == null) return;

            _propertiesMap.Add("major", _modifiedVersion.Major);
            _propertiesMap.Add("minor", _modifiedVersion.Minor);
            _propertiesMap.Add("patch", _modifiedVersion.Patch);
            _propertiesMap.Add("revision", _modifiedVersion.Revision);
            
            var content = ReleasingProjectManager
                                .Current
                                .VAParsingManager
                                .ModifyVersionAttributeOfOriginText(_propertiesMap);
            
            File.WriteAllText(_folderPath + "\\" + _versionFileName, content);
        }

        protected override bool IsTaskPossible(object param)
        {
            switch (param)
            {
                case object[] data:
                    if (data.Length == 3)
                    {
                        return !string.IsNullOrEmpty(data[0].ToString())
                       && Directory.Exists(data[0].ToString())
                       && !string.IsNullOrEmpty(data[1].ToString())
                       && File.Exists(data[0] + "\\" + data[1])
                       && data[2] is VersionPropertiesVO;
                    }
                    return false;

                default:
                    return false;
            }
        }
    }
}
