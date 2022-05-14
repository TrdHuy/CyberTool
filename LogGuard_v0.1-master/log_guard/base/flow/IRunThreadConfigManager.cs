using log_guard.models.vo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.@base.flow
{
    internal interface IRunThreadConfigManager
    {
        IRunThreadConfig CurrentConfig { get; }

        public LogParserVO CurrentParser { get; }

    }
}
