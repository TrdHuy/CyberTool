using LogGuard_v0._1.Base.LogGuardFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Implement.LogGuardFlow.RunThreadConfig
{
    public class RunThreadConfigImpl : IRunThreadConfig
    {
        private static RunThreadConfigImpl _instance;
        private LogParserVO _logParserFormat;
        public LogParserVO LogParserFormat { get => _logParserFormat; set => _logParserFormat = value; }

        public static RunThreadConfigImpl Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RunThreadConfigImpl();
                }
                return _instance;
            }
        }
    }
}
