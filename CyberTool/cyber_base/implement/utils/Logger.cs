using cyber_base.implement.attributes;
using cyber_base.utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cyber_base.implement.utils
{
    public class Logger : ILogger
    {
        private enum LogLv
        {
            [StringValue("V")]
            VERBOSE = 0,

            [StringValue("I")]
            INFO = 1,

            [StringValue("D")]
            DEBUG = 2,

            [StringValue("W")]
            WARNING = 3,

            [StringValue("F")]
            FATAL = 4,

            [StringValue("E")]
            ERROR = 5
        }

        private const string TAG = "CyberTool";
        private const int OLD_LOG_FILES_CAPACITY = 10;
        private static readonly SemaphoreSlim Mutex = new SemaphoreSlim(1);

        private static ObservableQueue<Task<bool>> TaskQueue { get; set; }
        private static StringBuilder? _logBuilder { get; set; }
        private static StringBuilder? _userLogBuilder { get; set; }
        private static string filePath { get; set; } = "";
        private static string fileName { get; set; } = "";
        private static string directory { get; set; } = "";
        private static string folderName { get; set; } = "";

        private string className { get; set; }
        private string moduleName { get; set; }
        private int PId { get; set; }
        private int TId { get; set; }

        static Logger()
        {

            TaskQueue = new ObservableQueue<Task<bool>>();
            var cast = TaskQueue as IEnumerable<Task<bool>>;
            ((INotifyCollectionChanged)cast).CollectionChanged += TaskQueueChanged;
#if DEBUG
            InitLogDebug();
#else
            InitUserLog();
#endif

            try
            {
                var dateTimeNow = DateTime.Now.ToString("ddMMyyHHmmss");
                var attribs = Assembly.GetCallingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), true);
                if (attribs.Length > 0)
                {
                    folderName = ((AssemblyCompanyAttribute)attribs[0]).Company 
                        + @"\"
                        + (Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly())
                            .GetName().Name
                        + @"\" 
                        + "log";
                }
                else
                {
                    folderName = TAG 
                        + @"\" 
                        + (Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly())
                            .GetName().Name 
                        + @"\" 
                        + "log";
                }

                fileName =
                    (Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly())
                        .GetName().Name 
                    + "_" +
                    (Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly())
                        .GetName().Version 
                    + "_" +
                    dateTimeNow + ".txt";

                directory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                directory = directory + @"\" + folderName;

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                filePath = directory + @"\" + fileName;

                AppDomain.CurrentDomain.ProcessExit -= CurrentDomain_ProcessExit;
                AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

                AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            }
            catch
            {
                try
                {
                    var dateTimeNow = DateTime.Now.ToString("ddMMyyHHmmss");
                    fileName =
                    Assembly.GetCallingAssembly().GetName().Name + "_" +
                    Assembly.GetCallingAssembly().GetName().Version + "_" +
                    dateTimeNow + ".txt";

                    directory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) + @"\" + "Data";

                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    filePath = directory + @"\" + fileName;

                    AppDomain.CurrentDomain.ProcessExit -= CurrentDomain_ProcessExit;
                    AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

                    AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
                    AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                }
                catch
                {

                }
            }

            DeleteLogInFolder();
        }

        /// <summary>
        /// Delete old logs 
        /// </summary>
        private static void DeleteLogInFolder()
        {
            try
            {
                var enumerateFile = Directory.EnumerateFiles(directory, "*.txt");
                List<string> fileNames = new List<string>(enumerateFile);
                var fileCount = enumerateFile.Count();
                if (fileCount > OLD_LOG_FILES_CAPACITY)
                {
                    for (int i = OLD_LOG_FILES_CAPACITY; i < fileCount; i++)
                    {
                        var temp = fileNames[i];
                        File.Delete(temp);
                    }
                }
            }
            catch
            {

            }

        }

        /// <summary>
        /// When a writting log task was push to a queue, process the queue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void TaskQueueChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                Task.Run(() => ProcessQueue());
            }
        }

        /// <summary>
        /// Fire a log when app fall into unhandle exception
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            WriteLog("F", TAG, "[UnhandledException]:" + e.ExceptionObject.ToString());
            var task1 = GenerateTask("F", TAG, "[UnhandledException]:" + e.ExceptionObject.ToString());
            TaskQueue.Enqueue(task1);
            var task2 = GenerateTask("", "", "", true);
            TaskQueue.Enqueue(task2);
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="className"></param>
        public Logger(string className, string moduleName = "cyber_base")
        {
            this.className = className;
            this.moduleName = moduleName;
            PId = Process.GetCurrentProcess().Id;
            TId = Thread.CurrentThread.ManagedThreadId;
        }

        private static void InitUserLog()
        {
            _userLogBuilder = new StringBuilder();
        }

        private static void InitLogDebug()
        {
            _logBuilder = new StringBuilder();
        }

        private static void CurrentDomain_ProcessExit(object? sender, EventArgs e)
        {
            ExportLogFile();
        }

        public void I(string message, [CallerMemberName] string callMemberName = "")
        {
            var task = GenerateTask("I", TAG, className, callMemberName, message);
            TaskQueue.Enqueue(task);
        }

        public void D(string message, [CallerMemberName] string callMemberName = "")
        {
            var task = GenerateTask("D", TAG, className, callMemberName, message);
            TaskQueue.Enqueue(task);
        }

        public void E(string message, [CallerMemberName] string callMemberName = "")
        {
            var task = GenerateTask("E", TAG, className, callMemberName, message);
            TaskQueue.Enqueue(task);
        }

        public void W(string message, [CallerMemberName] string callMemberName = "")
        {
            var task = GenerateTask("W", TAG, className, callMemberName, message);
            TaskQueue.Enqueue(task);
        }

        public void F(string message, [CallerMemberName] string callMemberName = "")
        {
            var task = GenerateTask("F", TAG, className, callMemberName, message);
            TaskQueue.Enqueue(task);
        }

        public void V(string message, [CallerMemberName] string callMemberName = "")
        {
            var task = GenerateTask("V", TAG, className, callMemberName, message);
            TaskQueue.Enqueue(task);
        }

        public void I(string message, string callMemberName, string callFilePath)
        {
            var task = GenerateTask("I", TAG, callFilePath, callMemberName, message, isTrimFilePath: true);
            TaskQueue.Enqueue(task);
        }

        public void D(string message, string callMemberName, string callFilePath)
        {
            var task = GenerateTask("D", TAG, callFilePath, callMemberName, message, isTrimFilePath: true);
            TaskQueue.Enqueue(task);
        }

        public void E(string message, string callMemberName, string callFilePath)
        {
            var task = GenerateTask("E", TAG, callFilePath, callMemberName, message, isTrimFilePath: true);
            TaskQueue.Enqueue(task);
        }

        public void W(string message, string callMemberName, string callFilePath)
        {
            var task = GenerateTask("W", TAG, callFilePath, callMemberName, message, isTrimFilePath: true);
            TaskQueue.Enqueue(task);
        }

        public void F(string message, string callMemberName, string callFilePath)
        {
            var task = GenerateTask("F", TAG, callFilePath, callMemberName, message, isTrimFilePath: true);
            TaskQueue.Enqueue(task);
        }

        public void V(string message, string callMemberName, string callFilePath)
        {
            var task = GenerateTask("V", TAG, callFilePath, callMemberName, message, isTrimFilePath: true);
            TaskQueue.Enqueue(task);
        }

        /// <summary>
        /// Process the queue when a task was pushed in
        /// do the task and remove it from queue if it is done
        /// or cancel if it try to do it three times
        /// </summary>
        private static async void ProcessQueue()
        {
            await Mutex.WaitAsync();
            try
            {
                int reDoWorkCounter = 0;

                while (TaskQueue.Count >= 1)
                {
                    var taskFormQueue = TaskQueue.Peek();
                    reDoWorkCounter++;
                    taskFormQueue.Start();
                    var success = taskFormQueue.Result;
                    if (success || reDoWorkCounter >= 3)
                    {
                        var removeTask = TaskQueue.Dequeue();
                        removeTask = null;
                        reDoWorkCounter = 0;
                    }
                }
            }
            finally
            {
                Mutex.Release();
            }
        }

        /// <summary>
        /// Generate a writting log task, to handle write log async
        /// </summary>
        /// <param name="logLV"></param>
        /// <param name="TAG"></param>
        /// <param name="className"></param>
        /// <param name="callMemberName"></param>
        /// <param name="message"></param>
        /// <param name="isExportLogFile"></param>
        /// <returns></returns>
        private Task<bool> GenerateTask(string logLV
            , string TAG
            , string className
            , string callMemberName
            , string message
            , bool isExportLogFile = false
            , bool isTrimFilePath = false)
        {
            var task = !isExportLogFile ?
                new Task<bool>(() =>
                {
                    return WriteLog(logLV, TAG, className, callMemberName, message, isTrimFilePath);
                }) :
             new Task<bool>(() =>
             {
                 var resExportLog = ExportLogFile();
                 return resExportLog;
             });

            return task;
        }

        /// <summary>
        /// Generate a writting log task, to handle write log async
        /// </summary>
        /// <param name="logLV"></param>
        /// <param name="TAG"></param>
        /// <param name="className"></param>
        /// <param name="callMemberName"></param>
        /// <param name="message"></param>
        /// <param name="isExportLogFile"></param>
        /// <returns></returns>
        private static Task<bool> GenerateTask(string logLV, string TAG, string message, bool isExportLogFile = false)
        {
            var task = !isExportLogFile ?
                new Task<bool>(() =>
                {
                    return WriteLog(logLV, TAG, message);
                }) :
            new Task<bool>(() =>
            {
                var resExportLog = ExportLogFile();
                return resExportLog;
            });
            return task;
        }

        /// <summary>
        /// Append the message log to log builder
        /// </summary>
        /// <param name="logLv"></param>
        /// <param name="tag"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private bool WriteLog(string logLv, string tag, string className, string methodName, string message, bool isTrimFilePath = false)
        {
            try
            {
                var newClassName = className;
                if (isTrimFilePath)
                {
                    var classFileName = className.Substring(className.LastIndexOf("\\") + 1);
                    newClassName = classFileName.Substring(0, classFileName.IndexOf("."));
                }

                var dateTimeNow = DateTime.Now.ToString("dd-MM HH:mm:ss:ffffff");
                var newLogLine = dateTimeNow + " " +
                    logLv + " " +
                    PId + " " +
                    TId + " " +
                    tag + " " +
                    moduleName + " " +
                    (newClassName == "" ? className : newClassName) + " " +
                    methodName + ":" + message;

                if (_logBuilder != null)
                {
                    _logBuilder.AppendLine(newLogLine);
                    ClearBuffer(_logBuilder);
                }

                if (_userLogBuilder != null)
                {
                    switch (logLv)
                    {
                        case "D":
                            break;
                        default:
                            _userLogBuilder.AppendLine(newLogLine);
                            break;
                    }
                    ClearBuffer(_userLogBuilder);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Append the message log to log builder
        /// </summary>
        /// <param name="logLv"></param>
        /// <param name="tag"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private static bool WriteLog(string logLv, string tag, string message)
        {
            try
            {
                var dateTimeNow = DateTime.Now.ToString("dd-MM HH:mm:ss:ffffff");
                var newLogLine = dateTimeNow + " " +
                    logLv + " " +
                    tag + " " +
                    message;

                if (_logBuilder != null)
                {
                    _logBuilder.AppendLine(newLogLine);
                    ClearBuffer(_logBuilder);
                }

                if (_userLogBuilder != null)
                {
                    switch (logLv)
                    {
                        case "D":
                            break;
                        default:
                            _userLogBuilder.AppendLine(newLogLine);
                            break;
                    }
                    ClearBuffer(_userLogBuilder);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// Clear the builder's buffer if reach max capacity 
        /// </summary>
        /// <param name="builder"></param>
        private static void ClearBuffer(StringBuilder builder)
        {
            if (builder.Capacity >= builder.MaxCapacity - 100000)
            {
                ExportLogFile();
                builder.Clear();
            }
        }

        /// <summary>
        /// Export log from string builder to file .txt
        /// </summary>
        /// <returns></returns>
        public static bool ExportLogFile()
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    File.Create(filePath).Dispose();
                }

                if (_logBuilder != null)
                {
                    File.AppendAllText(filePath, _logBuilder.ToString());
                }
                else if (_userLogBuilder != null)
                {
                    File.AppendAllText(filePath, _userLogBuilder.ToString());
                }
            }
            catch
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// return the directory of Pharmarcy's log files
        /// </summary>
        /// <returns></returns>
        public string GetLogDirectory()
        {
            return directory;
        }
    }

    internal class ObservableQueue<T> : Queue<T>, INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public new void Enqueue(T item)
        {
            base.Enqueue(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public new T Dequeue()
        {
            var x = base.Dequeue();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, x));
            return x;
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            RaiseCollectionChanged(e);
        }

        private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, e);
            }
        }
    }
}
