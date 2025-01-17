﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_installer.model
{
    /// <summary>
    /// Model sử dụng cho việc chuyển đổi dữ liệu giữa Cyber
    /// client và Cyber server
    /// Các property của model ở client sẽ tương đương với 
    /// các property của model trên server
    /// </summary>
    public class ToolVersionVO
    {
        public int VersionId { get; set; }
        public string Version { get; set; } = "";
        public string Description { get; set; } = "";
        public int ToolId { get; set; }
        public System.DateTime DatePublished { get; set; }
        public string FolderPath { get; set; } = "";
        public string FileName { get; set; } = "";
        public string AssemblyName { get; set; } = "";
        public string ExecutePath { get; set; } = "";
        public long CompressLength { get; set; }
        public long RawLength { get; set; }
    }
}
