using cyber_installer.@base.modules;
using cyber_installer.implement.modules.sw_installing_manager.http_requester;
using cyber_installer.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace cyber_installer.implement.modules.sw_installing_manager
{
    internal class SwInstallingManager : ISwInstallingManager
    {
        private SwDownloadRequester _swDownloadRequester;
        
        private SwInstallingManager()
        {
            _swDownloadRequester = new SwDownloadRequester();
        }

        public string GetInstallationPath()
        {
            throw new NotImplementedException();
        }

        public void OnModuleCreate()
        {
        }

        public void OnModuleDestroy()
        {
        }

        public void OnModuleStart()
        {
        }


        public async Task<bool> StartToolDownloadingTask(ToolVO toolVO)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    await _swDownloadRequester.RequestDownloadSoftware(client
                    , toolVO);
                    return true;
                }
                catch (HttpRequestException ex)
                {
                    if (ex.Message.Contains("No connection could be made because the target machine actively refused it"))
                    {
                    }
                }
                catch
                {

                }
                return false;

            }

        }

        public Task<bool> StartToolInstallingTask(ToolData toolData)
        {
            throw new NotImplementedException();
        }
    }
}
