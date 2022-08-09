using cyber_base.async_task;
using honeyboard_release_service.implement.log_manager;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace honeyboard_release_service.implement.ui_event_handler.async_tasks.io_tasks
{
    internal class FindVersionPropertiesFileTask : BaseRTParamAsyncTask
    {
        private readonly string[] VERSION_PROPERTIES_PARSER_SYNTAX = new string[] {
                    @".*major\s*=\s*(?<major>\d+).*\n.*minor\s*=\s*(?<minor>\d+).*\n.*patch\s*=\s*(?<patch>\d+).*(\n.*revision\s*=\s*(?<revision>\d+).*)*",
                    @"version\s*{(\r\n.*)*(\r\n\s*number\s+\""(?<major>\d+)\.(?<minor>\d+)(\.(?<patch>\d+)){0,1}(\.(?<revision>\d+)){0,1}\"")\s*(\r\n.*)*}",
                    @"version\s*{(\n.*)*(\n\s*number\s+\""(?<major>\d+)\.(?<minor>\d+)(\.(?<patch>\d+)){0,1}(\.(?<revision>\d+)){0,1}\"")\s*(\n.*)*}"
                };
        private readonly string[] VERSION_PROPERTIES_FILE_NAME = new string[] { "version.properties", "version.gradle" };
        private string[] _versionAttrFileName;
        private string[] _versionAttrParserSyntax;
        private string _projectPath;
        
        public FindVersionPropertiesFileTask(object param
            , Action<AsyncTaskResult>? completedCallback = null
            , string name = "Searching version properties file!") : base(param, name, completedCallback)
        {
            switch (param)
            {
                case object[] data:
                    if (data.Length == 3)
                    {
                        _versionAttrFileName = data[0] as string[] ?? VERSION_PROPERTIES_FILE_NAME;
                        _versionAttrParserSyntax = data[1] as string[] ?? VERSION_PROPERTIES_PARSER_SYNTAX;
                        _projectPath = data[2].ToString() ?? "";
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
            _delayTime = 2000;
            _reportDelay = 100;
        }

        protected override async Task DoAsyncMainTask(object param, AsyncTaskResult result, CancellationTokenSource token)
        {
            LogManager.Current.AppendLogLine("Searching version properties file!", true);

            if (Directory.Exists(_projectPath))
            {
                foreach (var fileName in _versionAttrFileName)
                {
                    var filePath = _projectPath + "\\" + fileName;

                    if (File.Exists(filePath))
                    {
                        var fileContent = await File.ReadAllTextAsync(filePath);

                        foreach (var syntax in _versionAttrParserSyntax)
                        {
                            if (Regex.IsMatch(fileContent, syntax, RegexOptions.Multiline))
                            {
                                dynamic final = new
                                {
                                    VersionFilePath = filePath,
                                    VersionSyntax = syntax,
                                    VersionFileContent = fileContent,
                                };
                                result.Result = final;
                                return;
                            }
                        }
                    }
                }
            }
        }

        protected override bool IsTaskPossible(object param)
        {
            switch (param)
            {
                case object[]:
                    return true;
                default:
                    return false;
            }
        }


    }
}
