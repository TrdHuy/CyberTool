using LogGuard_v0._1.Base.FileHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Implement.FileHelper
{
    public class FileHelperImpl : IFileHelper
    {
        public static FileHelperImpl _instance;

        public void ExportLinesToFile(string expPath, IEnumerable<string> lines)
        {
            try
            {
                File.WriteAllLines(expPath, lines);
            }
            catch (Exception ex)
            {
                App.Current.ShowErrorBox(ex.ToString());
            }
        }

        public void DeleteLogFile(string logPath)
        {
            try
            {
                File.Delete(logPath);
            }
            catch (Exception ex)
            {
                App.Current.ShowErrorBox(ex.ToString());
            }
        }

        public static FileHelperImpl Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FileHelperImpl();
                }
                return _instance;
            }
        }
    }
}
