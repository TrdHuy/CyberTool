using cyber_base.implement.utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.utils
{
    internal static class FileIOManager
    {
        private const string TAG = "LogGuard_v0.1";
        private const string DATA_FOLDER_NAME = "data";

        private static string directory { get; set; }
        private static string folderName { get; set; }
        private static string dataFolderName { get; set; }

        static FileIOManager()
        {
            try
            {
                var dateTimeNow = DateTime.Now.ToString("ddMMyyHHmmss");
                var attribs = Assembly.GetCallingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), true);
                if (attribs.Length > 0)
                {
                    folderName = ((AssemblyCompanyAttribute)attribs[0]).Company + @"\" + Assembly.GetCallingAssembly().GetName().Name;
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
            catch (Exception e)
            {
                try
                {
                    directory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
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

        public static T? LoadJsonFromDataFile<T>(string fileName)
        {
            var path = dataFolderName + fileName;
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }
            string json = File.ReadAllText(path, Encoding.UTF8);
            T? items = JsonHelper.DeserializeObject<T>(json);
            return items;
        }

        public static void ExportJsonToDataFile(string fileName, object obj)
        {
            var path = dataFolderName + fileName;
            var json = JsonHelper.SerializeObject(obj);
            File.WriteAllText(path, json);
        }
    }
}
