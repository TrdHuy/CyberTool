using cyber_base.async_task;
using cyber_base.implement.async_task;
using cyber_base.implement.utils;
using honeyboard_release_service.implement.module;
using honeyboard_release_service.implement.project_manager;
using honeyboard_release_service.models.VOs;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace honeyboard_release_service.implement.user_data_manager
{
    internal class UserDataManager : BasePublisherModule
    {
        private const string TAG = "h2sw_solution";
        private const string DATA_FOLDER_NAME = "data";
        private const string DATA_FILE_NAME = "user_data.json";
        private string directory = "";
        private string folderName = "";
        private string dataFolderName = "";
        private Dictionary<string, ProjectVO> _importedProjects;
        private ProjectVO? _currentImportedProject;
        private bool _isThisModuleLoaded = false;

        public static UserDataManager Current
        {
            get
            {
                return PublisherModuleManager.UDM_Instance;
            }
        }

        private UserDataManager()
        {
            _importedProjects = new Dictionary<string, ProjectVO>();
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

        public override async void OnModuleStart()
        {
            var loadUserDataTask = new CancelableAsyncTask(LoadUserData
                , new CancellationTokenSource()
                , null
                , async (result) =>
                {
                    _isThisModuleLoaded = true;
                    await Task.Delay(1);
                    ReleasingProjectManager
                        .Current
                        .UpdateWorkingProjectsAfterLoadedFromUserData(
                            _currentImportedProject
                            , _importedProjects);
                    return result;
                }
                , name: "Loading user data from json file");
            await loadUserDataTask.Execute();
        }

        public override async void OnDestroy()
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
            _importedProjects[projectPath] = projectVO;
        }

        public void SetCurrentImportedProject(ProjectVO projectVO)
        {
            _currentImportedProject = projectVO;
        }

        private async Task<AsyncTaskResult> ExportUserDataAsJson(
            CancellationTokenSource token
            , AsyncTaskResult result)
        {
            var path = dataFolderName + @"\" + DATA_FILE_NAME;
            dynamic obj = new ExpandoObject();
            obj.CurrentImportedProject = _currentImportedProject?.Path ?? "";
            obj.ImportedProjects = _importedProjects;
            var json = JsonHelper.SerializeObject(obj);
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
            dynamic? obj = JsonHelper.Decode(json);

            if (obj != null)
            {

                try
                {
                    _importedProjects = obj.ImportedProjects.ToObject<Dictionary<string, ProjectVO>>();
                }
                catch { }
                try
                {

                    var currentImportedPorjectPath = obj.CurrentImportedProject.ToObject<string>();
                    _currentImportedProject = _importedProjects[currentImportedPorjectPath];
                }
                catch { }
            }
            return result;
        }

    }
}
