using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;

namespace LogGuard_v0._1.Utils
{
    public class FileIOManager
    {
        private const string TAG = "LogGuard_v0.1";
        private const string DATA_FOLDER_NAME = "data";

        private static FileIOManager _instance;
        private static string directory { get; set; }
        private static string folderName { get; set; }
        private static string dataFolderName { get; set; }


        public static FileIOManager Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FileIOManager();
                }
                return _instance;
            }
        }

        private FileIOManager()
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

        public T LoadJsonFromDataFile<T>(string fileName)
        {
            var path = dataFolderName + fileName;
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }
            string json = File.ReadAllText(path, Encoding.UTF8);
            T items = JsonConvert.DeserializeObject<T>(json);
            return items;
        }

        public void ExportJsonToDataFile(string fileName, object obj)
        {
            var path = dataFolderName + fileName;
            var json = JsonConvert.SerializeObject(obj);
            File.WriteAllText(path, json);
        }
    }
}
