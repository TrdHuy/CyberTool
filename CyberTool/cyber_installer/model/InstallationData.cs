using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_installer.model
{
    /// <summary>
    /// Dữ liệu cài đặt này được lưu trữ tại thư mục cài đặt
    /// của mỗi phần mềm trên Cyber Installer
    /// Thư mục lưu trữ dữ liệu này thường được ẩn trong mục cài đặt
    /// của phần mềm, nó có tên là .h2sw
    /// </summary>
    internal class InstallationData
    {
        public string ToolKey { get; set; } = "";
        public string Guid { get; set; } = "";
        public string ToolName { get; set; } = "";
        public string CurrentInstalledVersion { get; set; } = "";
        public string InstallPath { get; set; } = "";
        public string ExecutePath { get; set; } = "";
        public string AssemblyName { get; set; } = "";
    }
}
