using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System;
using System.Text.RegularExpressions;

namespace cyber_build_task
{
#nullable enable
    public class InvokeCyberInstallerPackageBuilderTask : Task
    {
        public const string ExtractVersionInfoFromZipToJsonTaskKey = "ExtractVersionInfoFromZipToJson";
        public const string DeleteAllFileInFolderTaskKey = "DeleteAllFileInFolder";

        [Required]
        public string TaskKey { get; set; }

        [Required]
        public string Args { get; set; }


        public InvokeCyberInstallerPackageBuilderTask()
        {
            TaskKey = "";
            Args = "";
        }

        public override bool Execute()
        {
            Log.LogMessageFromText("InvokeCyberInstallerPackageBuilderTask: TaskKey: " + TaskKey, MessageImportance.High);
            Log.LogMessageFromText("InvokeCyberInstallerPackageBuilderTask: Args: " + Args, MessageImportance.High);

            var propertyMapper = CreatePropertyMapperFromRequestArg(Args);
            ICyberInstallerPackageBuilderTask? mainTask = null;
            switch (TaskKey)
            {
                case DeleteAllFileInFolderTaskKey:
                    {
                        mainTask = CreateTask<DeleteAllFileInFolderTask>(TaskKey, propertyMapper, Log);
                        goto default;
                    }
                case ExtractVersionInfoFromZipToJsonTaskKey:
                    {
                        mainTask = CreateTask<ExtractVersionPackageInfoTask>(TaskKey, propertyMapper, Log);
                        goto default;
                    }
                default:
                    if (mainTask == null)
                    {
                        Log.LogError("TaskKey: " + TaskKey + " not found!");
                        return false;
                    }
                    else
                    {
                        Log.LogMessageFromText("InvokeCyberInstallerPackageBuilderTask: Start task executing: " + TaskKey, MessageImportance.High);
                        mainTask.Execute();
                        return true;
                    }
            }
        }

        private static Dictionary<string, string> CreatePropertyMapperFromRequestArg(string requestArg)
        {
            string argNameGroupId = "ArgName";
            string argValueGroupId = "ArgValue";
            string allowedSpecialChars = @"=.,\s:\\_";
            var pattern = $"(?<{argNameGroupId}>[a-zA-Z][a-zA-Z0-9]*):'(?<{argValueGroupId}>[a-zA-Z0-9{allowedSpecialChars}]*)';";

            var propertyMapper = new Dictionary<string, string>();
            Regex argParser = new Regex(pattern);
            var argMatches = argParser.Matches(requestArg);

            if (argMatches.Count > 0)
            {
                for (int i = 0; i < argMatches.Count; i++)
                {
                    propertyMapper.Add(argMatches[i].Groups[argNameGroupId].Value,
                        argMatches[i].Groups[argValueGroupId].Value);
                }
            }

            return propertyMapper;
        }

        private static T CreateTask<T>(string request
            , Dictionary<string, string> propertyMapper
            , TaskLoggingHelper tLH) where T : class, ICyberInstallerPackageBuilderTask
        {
            var taskType = typeof(T);
            var obj = Activator.CreateInstance(taskType, tLH) ?? throw new InvalidOperationException($"Cannot create instance for task:{request}");
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(obj);
            foreach (PropertyDescriptor property in properties)
            {
                var attr = property.Attributes;
                var requireAttr = attr[typeof(RequiredAttribute)] as RequiredAttribute;
                var propName = property.Name;

                if (requireAttr != null)
                {
                    if (!propertyMapper.ContainsKey(propName))
                    {
                        throw new ArgumentNullException($"Require property:{propName} for task:{request}");
                    }
                    property.SetValue(obj, propertyMapper[propName]);
                }
            }
            return (T)obj;
        }
    }
#nullable disable
}
