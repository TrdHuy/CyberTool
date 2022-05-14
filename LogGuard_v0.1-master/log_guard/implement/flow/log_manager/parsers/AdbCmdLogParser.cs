using log_guard.@base._log.android_log;
using log_guard.models.info;
using log_guard.models.info.builder;
using System.Text.RegularExpressions;

namespace log_guard.implement.flow.log_manager.parsers
{
    public class AdbCmdLogParser : AbstractLogParser
    {
        protected override void InitDataGroupParser()
        {
            DateGroup = @"(?<Date>[0-1]\d-[0-3]\d)" + @"\s+";
            TimeGroup = @"(?<Time>[0-2]\d:[0-6]\d:[0-6]\d.\d+)" + @"\s+";
            PidGroup = @"(?<Pid>\w+)" + @"\s+";
            TidGroup = @"(?<Tid>\w+)" + @"\s+";
            LevelGroup = @"(?<Level>[VDIWEF])" + @"\s+";
            TagGroup = @"(?<Tag>\S+)" + @"\s+";
            MessageGroup = @"(?<Message>.*)";
            Pattern = DateGroup + TimeGroup + PidGroup + TidGroup + LevelGroup + TagGroup + MessageGroup;
            
            // Web pattern version:
            // (?P<Date>[0-1]\d-[0-3]\d)\s+(?P<Time>[0-2]\d:[0-6]\d:[0-6]\d.\d+)\s+(?P<Pid>\w+)\s+(?P<Tid>\w+)\s+(?P<Level>[VDIWEF])\s+(?P<Tag>\S+)\s+(?P<Message>.*)
        }

        public override LogInfo ParseLogInfos(string line, int lineNumber)
        {
            LogBuilder builder = new LogBuilder();

            Match match = Regex.Match(line, Pattern);

            if (Regex.IsMatch(line, Pattern))
            {
                builder.BuildDate(match.Groups[LogInfo.KEY_DATE].ToString())
                        .BuildTime(match.Groups[LogInfo.KEY_TIME].ToString())
                        .BuildDateTime()
                        .BuildTID(match.Groups[LogInfo.KEY_TID].ToString())
                        .BuildPID(match.Groups[LogInfo.KEY_PID].ToString())
                        .BuildColorByLevel(match.Groups[LogInfo.KEY_LEVEL].ToString())
                        .BuildTag(match.Groups[LogInfo.KEY_TAG].ToString())
                        .BuildMessage(match.Groups[LogInfo.KEY_MESSAGE].ToString())
                        .BuildLine(lineNumber)
                        .BuildRawText(line);
                return builder.Build();
            }

            return null;
        }
    }
}