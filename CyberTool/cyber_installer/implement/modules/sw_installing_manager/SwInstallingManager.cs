using cyber_base.async_task;
using cyber_base.implement.utils;
using cyber_installer.@base;
using cyber_installer.@base.modules;
using cyber_installer.implement.modules.sw_installing_manager.http_requester;
using cyber_installer.implement.modules.ui_event_handler.async_task;
using cyber_installer.implement.modules.utils;
using cyber_installer.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace cyber_installer.implement.modules.sw_installing_manager
{
    internal class SwInstallingManager : BaseCyberInstallerModule, ISwInstallingManager
    {
        private Logger _logger = new Logger("SwInstallingManager", "cyber_installer");
        private SwDownloadRequester _swDownloadRequester;
        private SwInstallingManager()
        {
            _swDownloadRequester = new SwDownloadRequester();
        }

        public static SwInstallingManager Current
        {
            get => ModuleManager.SIM_Instance;
        }

        public string GetInstallationPath()
        {
            throw new NotImplementedException();
        }

        public async Task<ToolData?> StartDownloadingLatestVersionToolTask(ToolVO toolVO
            , Action<object, double> downloadProgressChangedCallback)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var data = await _swDownloadRequester.Request(client
                    , toolVO
                    , downloadProgressChangedCallback);
                    _logger.I("Sucessfully to request download latest version of " + toolVO.Name);
                    return data;
                }
                catch (HttpRequestException ex)
                {
                    if (ex.Message.Contains("No connection could be made because the target machine actively refused it"))
                    {
                        App.Current.ShowErrorBox("Can not connect to server!\n" + ex.Message);
                    }
                    _logger.E("Fail to request download latest version of " + toolVO.Name);
                    _logger.E(ex.Message);
                }
                catch (Exception ex)
                {
                    App.Current.ShowErrorBox("Fail to request download latest version of " + toolVO.Name + "!\n" + ex.Message);
                    _logger.E("Fail to request download latest version of " + toolVO.Name);
                    _logger.E(ex.Message);
                }
                return null;
            }
        }

        public async Task<ToolData?> StartToolInstallingTask(ToolData toolData
            , string installPath
            , Action<double>? installProgressChangedCallback = null)
        {
            var installTask = new InstallSoftwareTask(toolData
                , installPath
                , callback: (res) =>
                {
                    if (res.MesResult == MessageAsyncTaskResult.Aborted
                        || res.MesResult == MessageAsyncTaskResult.Faulted)
                    {
                        _logger.E(res.Messsage);
                    }
                    else
                    {
                        _logger.I("Install " + toolData.StringId + " successfully at " + installPath);
                    }
                });
            installTask.ProgressChanged += (s, e2) =>
            {
                installProgressChangedCallback?.Invoke(e2);
            };
            await installTask.Execute();

            return toolData;
        }

        public async Task StartUninstallToolTask(ToolData toolData, Action<object, double>? progressChangedCallback = null)
        {
            var installTask = new UninstallSoftwareTask(toolData
                 , callback: (res) =>
                 {
                     if (res.MesResult == MessageAsyncTaskResult.Aborted
                         || res.MesResult == MessageAsyncTaskResult.Faulted)
                     {
                         _logger.E(res.Messsage);
                     }
                     else
                     {
                         toolData.ToolStatus = ToolStatus.Removed;
                     }
                 });
            installTask.ProgressChanged += (s, e2) =>
            {
                progressChangedCallback?.Invoke(s, e2);
            };
            await installTask.Execute();
        }
    }
}
