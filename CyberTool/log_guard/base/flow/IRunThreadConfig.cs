using log_guard.models;
using log_guard.models.vo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.@base.flow
{
    internal interface IRunThreadConfig
    {
        LogParserVO LogParserFormat { get; set; }
        List<TrippleToggleItemVO> LogTags { get; set; }
        List<TrippleToggleItemVO> LogMessages { get; set; }
    }
}
