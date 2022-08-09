using cyber_base.async_task;
using honeyboard_release_service.implement.log_manager;
using honeyboard_release_service.models.VOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace honeyboard_release_service.implement.ui_event_handler.async_tasks.io_tasks
{
    internal class ModifyVersionPropertiesFileTask : BaseRTParamAsyncTask
    {
        private string _folderPath = "";
        private string _versionFileName = "";
        private VersionPropertiesVO? _modifiedVersion;

        //TODO : Sử dụng version parser manager thay cho bộ parser cũ 
        private static readonly Regex _majorRegex
            = new Regex(@"(?<front>\s*(?<property>major)=)(?<value>\d+)");
        private static readonly Regex _minorRegex
            = new Regex(@"(?<front>\s*(?<property>minor)=)(?<value>\d+)");
        private static readonly Regex _patchRegex
            = new Regex(@"(?<front>\s*(?<property>patch)=)(?<value>\d+)");
        private static readonly Regex _revisionRegex
            = new Regex(@"(?<front>\s*(?<property>revision)=)(?<value>\d+)");

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

            if (File.Exists(_folderPath + "\\" + _versionFileName))
            {
                string[] lines = File.ReadAllLines(_folderPath + "\\" + _versionFileName);

                for (int i = 0; i < lines.Length; i++)
                {
                    if (_majorRegex.IsMatch(lines[i])
                        && !string.IsNullOrEmpty(_modifiedVersion?.Major))
                    {
                        lines[i] = _majorRegex.Replace(lines[i], (m) =>
                        {
                            return m.Groups["front"] + _modifiedVersion.Major;
                        });
                    }
                    else if (_minorRegex.IsMatch(lines[i])
                        && !string.IsNullOrEmpty(_modifiedVersion?.Minor))
                    {
                        lines[i] = _minorRegex.Replace(lines[i], (m) =>
                        {
                            return m.Groups["front"] + _modifiedVersion.Minor;
                        });
                    }
                    else if (_patchRegex.IsMatch(lines[i])
                        && !string.IsNullOrEmpty(_modifiedVersion?.Patch))
                    {
                        lines[i] = _patchRegex.Replace(lines[i], (m) =>
                        {
                            return m.Groups["front"] + _modifiedVersion.Patch;
                        });
                    }
                    else if (_revisionRegex.IsMatch(lines[i])
                        && !string.IsNullOrEmpty(_modifiedVersion?.Revision))
                    {
                        lines[i] = _revisionRegex.Replace(lines[i], (m) =>
                        {
                            return m.Groups["front"] + _modifiedVersion.Revision;
                        });
                    }
                }

                File.WriteAllText(_folderPath + "\\" + _versionFileName, string.Join("\n", lines));
            }
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
