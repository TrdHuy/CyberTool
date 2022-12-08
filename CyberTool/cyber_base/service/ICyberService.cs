using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_base.service
{
    public interface ICyberService
    {
        /// <summary>
        /// 
        /// </summary>
        ICyberServiceManager? ServiceManager { get; }

        /// <summary>
        /// 
        /// </summary>
        bool IsUnderconstruction { get; }

        /// <summary>
        /// 
        /// </summary>
        string ServiceID { get; }

        /// <summary>
        /// 
        /// </summary>
        long ServicePageLoadingDelayTime { get; }


        /// <summary>
        /// 
        /// </summary>
        string HeaderGeometryData { get; }

        /// <summary>
        /// 
        /// </summary>
        string Header { get; }

        /// <summary>
        /// 
        /// </summary>
        object? GetServiceView();
        
        /// <summary>
        /// 
        /// </summary>
        Uri ServiceResourceUri { get; }
        
        /// <summary>
        /// Sự kiện này xảy ra khi service được khởi tạo lần đầu tiên
        /// Thường sẽ được khởi tạo trước khi vào app start up 
        /// </summary>
        /// <param name="cyberServiceManager"> Người khởi tạo service</param>
        void OnServiceCreate(ICyberServiceManager cyberServiceManager);

        /// <summary>
        /// Sự kiện này xảy ra khi tắt app
        /// </summary>
        /// <param name="cyberServiceManager"> Người khởi tạo service</param>
        void OnServiceDestroy(ICyberServiceManager cyberServiceManager);

        /// <summary>
        /// Sự kiện này xảy ra khi service bắt đầu khởi tạo nội dung.
        /// </summary>
        /// <param name="cyberServiceManager"> Người khởi tạo service</param>
        void OnPreServiceViewInit(ICyberServiceManager cyberServiceManager);

        /// <summary>
        /// Sự kiện này xảy ra khi service khởi tạo nội dung hoàn tất.
        /// </summary>
        /// <param name="cyberServiceManager"> Người khởi tạo service</param>
        void OnServiceViewInstantiated(ICyberServiceManager cyberServiceManager);

        /// <summary>
        /// Sự kiện này xảy ra khi service hiển thị nội dung hoàn tất
        /// </summary>
        /// <param name="cyberServiceManager"> Người khởi tạo service</param>
        void OnServiceViewLoaded(ICyberServiceManager cyberServiceManager);

        /// <summary>
        /// Sự kiện này xảy ra khi người dùng chuyển sang service khác.
        /// </summary>
        /// <param name="cyberServiceManager"> Người khởi tạo service</param>
        void OnServiceUnloaded(ICyberServiceManager cyberServiceManager);
    }
}
