using cyber_base.async_task;
using cyber_base.implement.async_task;
using cyber_base.implement.utils;
using progtroll.implement.module;
using progtroll.implement.project_manager;
using progtroll.models.UDs;
using progtroll.models.VOs;
using progtroll.view_models.tab_items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace progtroll.implement.user_data_manager
{
    internal class UserDataManager
    {
        private const string TAG = "h2sw_solution";
        private const string DATA_FOLDER_NAME = "data";
        private const string DATA_FILE_NAME = "user_data.json";

        private string directory = "";
        private string folderName = "";
        private string dataFolderName = "";

        private RWableJsonUD _rWableJsonUD;

        private bool _isThisModuleLoaded = false;

        public UserDataManager()
        {
            _rWableJsonUD = new RWableJsonUD();
            try
            {
                var dateTimeNow = DateTime.Now.ToString("ddMMyyHHmmss");
                var attribs = Assembly.GetCallingAssembly()
                    .GetCustomAttributes(typeof(AssemblyCompanyAttribute), true);
                if (attribs.Length > 0)
                {
                    folderName = ((AssemblyCompanyAttribute)attribs[0]).Company
                        + @"\" + Assembly.GetCallingAssembly().GetName().Name;
                }
                else
                {
                    folderName = TAG + @"\" + Assembly.GetCallingAssembly().GetName().Name;
                }

                directory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                directory = directory + @"\" + folderName;
                dataFolderName = directory + @"\" + DATA_FOLDER_NAME;
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                if (!Directory.Exists(dataFolderName))
                {
                    Directory.CreateDirectory(dataFolderName);
                }
            }
            catch
            {
                try
                {
                    directory = Path.GetDirectoryName(Assembly
                        .GetEntryAssembly()?
                        .Location ?? "") ?? "";
                    dataFolderName = directory + @"\" + DATA_FOLDER_NAME;

                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    if (!Directory.Exists(dataFolderName))
                    {
                        Directory.CreateDirectory(dataFolderName);
                    }
                }
                catch
                {

                }
            }
        }

        public async void OnInit()
        {
            var loadUserDataTask = new CancelableAsyncTask(LoadUserData
                , new CancellationTokenSource()
                , null
                , async (result) =>
                {
                    _isThisModuleLoaded = true;
                    await Task.Delay(1);

                    if (_rWableJsonUD != null)
                    {
                        ReleasingProjectManager
                            .Current
                            .UpdateWorkingProjectsAfterLoadedFromUserData(
                                _rWableJsonUD);
                    }

                    return result;
                }
                , name: "Loading user data from json file");

            await loadUserDataTask.Execute();
        }

        public async void OnDestroy()
        {
            if (_isThisModuleLoaded)
            {
                var exportTask = new CancelableAsyncTask(ExportUserDataAsJson
               , new CancellationTokenSource()
               , name: "Exporting user data as Json");
                await exportTask.Execute();
            }
        }

        public void AddImportedProject(string projectPath, ProjectVO projectVO)
        {
            if (_rWableJsonUD != null)
            {
                _rWableJsonUD.ImportProjects[projectPath] = projectVO;
            }

        }

        public void SetCurrentImportedProject(ProjectVO? projectVO)
        {
            _rWableJsonUD.CurrentImportedProjectPath = projectVO?.Path ?? "";
        }

        public void AddReleaseTemplateItemSource(ReleaseTemplateUD item)
        {
            _rWableJsonUD.ReleaseTemplateSource.Add(item);
        }

        private async Task<AsyncTaskResult> ExportUserDataAsJson(
            CancellationTokenSource token
            , AsyncTaskResult result)
        {
            var path = dataFolderName + @"\" + DATA_FILE_NAME;

            var json = JsonHelper.SerializeObject(_rWableJsonUD);
            await File.WriteAllTextAsync(path, json);

            return result;
        }

        private async Task<AsyncTaskResult> LoadUserData(
           CancellationTokenSource token
           , AsyncTaskResult result)
        {
            var path = dataFolderName + @"\" + DATA_FILE_NAME;

            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }

            string json = await File.ReadAllTextAsync(path, Encoding.UTF8);

            _rWableJsonUD = JsonHelper.DeserializeObject<RWableJsonUD>(json) ?? new RWableJsonUD();

            return result;
        }

    }
}
