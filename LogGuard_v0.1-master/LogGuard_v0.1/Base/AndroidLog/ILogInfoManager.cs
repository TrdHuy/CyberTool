using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Windows.MainWindow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Base.Log
{
    interface ILogInfoManager
    {
        /**
         * Cập nhật List chứa các LogInfo, List này được dùng để
         * lưu các giá trị LogInfo ghi được từ device
         * 
         */
        void ResetLogInfos();

        /**
         * Đưa Log từ dạng String về các tham số, trạng thái 
         * đọc log từ File hay Log run
         * và trả lại 1 đối tượng LogInfo
         * 
         */
        LogInfo ParseLogInfos(string lines, bool isReadFromFile, bool isReadFromDumpstateFile);

        /*
         * Cập nhật LogParser cho phù hợp với lệnh cmd hiện tại
         * cũng như phù hợp với từng loại log hiện tại mà người
         * dùng muốn hiển thị
         * 
         */
        void UpdateLogParser();

        /*
         * Trả về giá trị list các loginfo đã parse được theo level
         * tính từ lúc bắt đầu parse (sau khi click Run lần đầu và Run sau khi Stop)
         * 
         */
        List<LogInfo> GetLogInfosByLevel(string level);

        /*
         * Trả về giá trị observable collection các log item đã parse được
         * tính từ lúc bắt đầu parse (sau khi click Run lần đầu và Run sau khi Stop)
         * 
         */
        List<BaseViewModel> GetLogItemVMs();
    }
}
