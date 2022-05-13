using log_guard.definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.models.vo
{
    internal class LogParserVO
    {
        public string DisplayName { get; set; }
        public string ParserTip { get; set; }
        public string Cmd { get; set; }
        public LogParserFormatContact FormatContact { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
