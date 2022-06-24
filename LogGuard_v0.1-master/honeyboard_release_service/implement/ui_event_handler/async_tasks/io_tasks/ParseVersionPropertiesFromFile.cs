using cyber_base.async_task;
using honeyboard_release_service.implement.log_manager;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace honeyboard_release_service.implement.ui_event_handler.async_tasks.io_tasks
{
    internal class ParseVersionPropertiesFromFile : BaseRTParamAsyncTask
    {
        private string _folderPath;
        private string _versionFileName;
        private static readonly Regex _majorRegex = new Regex(@"\s*(?<property>major)=(?<value>\d+)");
        private static readonly Regex _minorRegex = new Regex(@"\s*(?<property>minor)=(?<value>\d+)");
        private static readonly Regex _patchRegex = new Regex(@"\s*(?<property>patch)=(?<value>\d+)");
        private static readonly Regex _revisionRegex = new Regex(@"\s*(?<property>revision)=(?<value>\d+)");
        private Dictionary<string, string> _propertiesMap = new Dictionary<string, string>();

        public ParseVersionPropertiesFromFile(object param
             , Action<AsyncTaskResult>? completedCallback = null
             , string name = "Parse version properties from file")
             : base(param, name, completedCallback)
        {
            _versionFileName = "";
            _folderPath = "";
            switch (param)
            {
                case string[] data:
                    if (data.Length == 2)
                    {
                        _folderPath = data[0];
                        _versionFileName = data[1];
                    }
                    break;
                default:
                    throw new InvalidDataException("Param must be an array of string has 2 elements");
            }

            _estimatedTime = 300;
            _reportDelay = 100;
            _delayTime = 300;
        }


        protected override void DoCallback(object param, AsyncTaskResult result)
        {
            result.Result = _propertiesMap;
        }

        protected override void DoMainTask(object param, AsyncTaskResult result, CancellationTokenSource token)
        {
            LogManager.Current.AppendLogLine("Parse version properties from file", true);

            if (File.Exists(_folderPath + "\\" + _versionFileName))
            {
                foreach (string line in File.ReadLines(_folderPath + "\\" + _versionFileName))
                {
                    Regex? selectedRegex = null;
                    if (_majorRegex.IsMatch(line))
                    {
                        selectedRegex = _majorRegex;
                    }
                    else if (_minorRegex.IsMatch(line))
                    {
                        selectedRegex = _minorRegex;
                    }
                    else if (_patchRegex.IsMatch(line))
                    {
                        selectedRegex = _patchRegex;
                    }
                    else if (_revisionRegex.IsMatch(line))
                    {
                        selectedRegex = _revisionRegex;
                    }

                    if (selectedRegex != null)
                    {
                        var match = selectedRegex.Match(line);
                        _propertiesMap.Add(match.Groups["property"].ToString(), match.Groups["value"].ToString());
                    }
                }
            }
        }

        protected override bool IsTaskPossible(object param)
        {
            switch (param)
            {
                case string[] data:
                    if (data.Length == 2)
                    {
                        return !string.IsNullOrEmpty(data[0])
                       && Directory.Exists(data[0])
                       && !string.IsNullOrEmpty(data[1])
                       && File.Exists(data[0] + "\\" + data[1]);
                    }
                    return false;

                default:
                    return false;
            }
        }
    }
}
