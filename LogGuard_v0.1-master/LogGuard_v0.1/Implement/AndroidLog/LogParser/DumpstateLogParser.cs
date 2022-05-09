using LogGuard_v0._1.Base.AndroidLog;
using LogGuard_v0._1.Base.AndroidLog.LogParser;
using LogGuard_v0._1.Windows.MainWindow.Models.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Implement.AndroidLog.LogParser
{
    public class DumpstateLogParser : AbstractLogParser
    {
        protected override void InitDataGroupParser()
        {
            DateGroup = @"(?<Date>[0-1]\d-[0-3]\d)" + @"\s+";
            TimeGroup = @"(?<Time>[0-2]\d:[0-6]\d:[0-6]\d.\d+)" + @"\s+";
            UidGroup = @"(?<Uid>\w+)" + @"\s+";
            PidGroup = @"(?<Pid>\w+)" + @"\s+";
            TidGroup = @"(?<Tid>\w+)" + @"\s+";
            LevelGroup = @"(?<Level>[VDIWEF])" + @"\s+";
            TagGroup = @"(?<Tag>\S+)" + @"\s+";
            MessageGroup = @"(?<Message>.*)";
            Pattern = DateGroup + TimeGroup + UidGroup + PidGroup + TidGroup + LevelGroup + TagGroup + MessageGroup;
            Pattern2 = DateGroup + TimeGroup + PidGroup + TidGroup + LevelGroup + TagGroup + MessageGroup;

            //Pattern = @"(\d{2}-\d{2})\s*(\d{2}:\d{2}:\d{2}\.\d{3})\s*((\d+)\s*(\d+)\s*(\d+|\w*))\s*(\w)\s*((\w*)\s*\:)\s*(.*)";
        }

        public override LogInfo ParseLogInfos(string line, int lineNumber)
        {
            LogBuilder builder = new LogBuilder();

            MatchCollection matches = Regex.Matches(line, Pattern);
            if (matches.Count == 0 && Pattern2.Length > 0)
                matches = Regex.Matches(line, Pattern2);

            foreach (Match match in matches)
            {
                builder.Reset();
                builder.BuildLine(lineNumber)
                    .BuildDate(match.Groups[LogInfo.KEY_DATE].ToString())
                    .BuildTime(match.Groups[LogInfo.KEY_TIME].ToString())
                    .BuildDateTime()
                    .BuildPID(match.Groups[LogInfo.KEY_PID].ToString())
                    .BuildTID(match.Groups[LogInfo.KEY_TID].ToString())
                    .BuildMessage(match.Groups[LogInfo.KEY_MESSAGE].ToString())
                    .BuildTag(match.Groups[LogInfo.KEY_TAG].ToString())
                    .BuildColorByLevel(match.Groups[LogInfo.KEY_LEVEL].ToString());
                return builder.Build();
            }

            return null;
        }
    }
}
