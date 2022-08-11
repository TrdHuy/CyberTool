using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Utils
{
    class ProcessManagement
    {
        private static ProcessManagement _processManagement;
        private List<int> _processIDList = new List<int>();

        public static ProcessManagement GetInstance()
        {
            if (_processManagement == null)
                _processManagement = new ProcessManagement();
            return _processManagement;
        }

        public void AddNewProcessID(int id)
        {
            _processIDList.Add(id);
        }

        public void KillAllProcess()
        {
            foreach (int p in _processIDList)
            {
                KillProcessAndChildren(p);
            }
        }

        private void KillProcessAndChildren(int pid)
        {
            // Cannot close 'system idle process'.
            if (pid == 0)
            {
                return;
            }
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + pid);
            ManagementObjectCollection moc = searcher.Get();
            foreach (ManagementObject mo in moc)
            {
                KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
            }
            try
            {
                Process proc = Process.GetProcessById(pid);
                proc.Kill();
                proc.Dispose();
                proc.Close();
            }
            catch (ArgumentException)
            {
                // Process already exited.
            }
        }
    }
}
