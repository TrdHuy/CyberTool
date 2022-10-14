using cyber_base.async_task;
using cyber_base.definition;
using cyber_base.implement.async_task;
using cyber_base.utils;
using cyber_base.view_model;
using progtroll.implement.project_manager;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace progtroll.implement.ui_event_handler.actions.project_manager.gesture
{
    internal class PRT_PM_VersionFilePathSelectedAction : PM_ViewModelCommandExecuter
    {
        public PRT_PM_VersionFilePathSelectedAction(string actionID, string builderID, object? dataTransfer, BaseViewModel viewModel, ILogger? logger)
            : base(actionID, builderID, dataTransfer, viewModel, logger)
        {
        }

        protected override void ExecuteCommand()
        {
            if (DataTransfer != null)
            {
                var path = DataTransfer[0].ToString() ?? "";
                var versionAttrParserSyntax = ReleasingProjectManager.Current.VAParsingManager.GetVersionPropertiesParserMainSyntax();

                CancelableAsyncTask openVersionFile = new CancelableAsyncTask(
                    mainFunc: async (cts, res) =>
                    {
                        var fileContent = await File.ReadAllTextAsync(path);

                        foreach (var syntax in versionAttrParserSyntax)
                        {
                            if (Regex.IsMatch(fileContent, syntax, RegexOptions.Multiline))
                            {
                                ReleasingProjectManager.Current
                                    .SetVersionAttrFilePathAndSyntaxOfCurrentImportProject(path, syntax, fileContent);

                                return res;
                            }
                        }

                        return res;
                    }
                    , cancellationTokenSource: new CancellationTokenSource()
                    , estimatedTime: 2000
                    , delayTime: 2000
                    , name: "Parsing version properties file");

                List<BaseAsyncTask> tasks = new List<BaseAsyncTask>();
                tasks.Add(openVersionFile);

                MultiAsyncTask multiTask = new MultiAsyncTask(mainFunc: tasks
                    , cancellationTokenSource: new CancellationTokenSource()
                    , name: "Parsing version properties file"
                    , delayTime: 0
                    , reportDelay: 100);

                var message = ProgTroll.Current.ServiceManager?.App.OpenMultiTaskBox("Parsing version properties file", multiTask);

                if (message != CyberContactMessage.Cancel
                    && PMViewModel.VersionPropertiesFileName != "")
                {
                    ReleasingProjectManager.Current.UpdateVersionHistoryTimelineInBackground();
                }
                else if (message == CyberContactMessage.Cancel)
                {
                }
            }
        }
    }
}
