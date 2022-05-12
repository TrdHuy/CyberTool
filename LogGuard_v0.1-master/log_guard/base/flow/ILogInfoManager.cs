using log_guard.@base.flow;
using log_guard.models;

namespace log_guard.@base.flow
{
    public interface ILogInfoManager
    {

        /// <summary>
        /// Cập nhật List chứa các LogInfo, List này được dùng để
        /// lưu các giá trị LogInfo ghi được từ device
        /// </summary>
        void ResetLogInfos();

        /// <summary>
        ///  Đưa Log từ dạng String về các tham số, trạng thái 
        /// đọc log từ File hay Log run
        /// và trả lại 1 đối tượng LogInfo
        /// </summary>
        LogInfo ParseLogInfos(string lines, bool isReadFromFile, bool isReadFromDumpstateFile);

        /// <summary>
        /// Cập nhật LogParser cho phù hợp với lệnh cmd hiện tại
        /// cũng như phù hợp với từng loại log hiện tại mà người
        /// dùng muốn hiển thị
        /// </summary>
        void UpdateLogParser(IRunThreadConfig runThreadConfig);

    }
}
