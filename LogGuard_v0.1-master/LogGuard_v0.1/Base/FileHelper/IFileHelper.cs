using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Base.FileHelper
{
    public interface IFileHelper
    {
        void ExportLinesToFile(string expPath, IEnumerable<string> lines);

        void DeleteLogFile(string logPath);
    }
}
