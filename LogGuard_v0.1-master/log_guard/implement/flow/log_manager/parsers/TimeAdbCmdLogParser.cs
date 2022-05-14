﻿using log_guard.@base._log.android_log;
using log_guard.models.info;
using log_guard.models.info.builder;
using System.Text.RegularExpressions;

namespace log_guard.implement.flow.log_manager.parsers
{
    public class TimeAdbCmdLogParser : AbstractLogParser
    {
        protected override void InitDataGroupParser()
        {
            DateGroup = @"(?<Date>[0-1]\d-[0-3]\d)" + @"\s+";
            TimeGroup = @"(?<Time>[0-2]\d:[0-6]\d:[0-6]\d.\d+)" + @"\s+";
            LevelGroup = @"(?<Level>[VDIWEF])" + @"/";
            TagGroup = @"(?<Tag>\S+)" + @"[(]\s*";
            PidGroup = @"(?<Pid>\w+)" + @"[)]:";
            MessageGroup = @"(?<Message>.*)";
            Pattern = DateGroup + TimeGroup + LevelGroup + TagGroup + PidGroup + MessageGroup;

            // Web pattern version:
            //(? P<Date>[0 - 1]\d -[0 - 3]\d)\s + (? P<Time>[0 - 2]\d:[0 - 6]\d:[0 - 6]\d.\d +)\s + (? P<Level>[VDIWEF])\/ (? P < Tag >\S +)[(]\s * (? P < Pid >\w +)[)]:(? P<Message>.*)
        }

        public override LogInfo ParseLogInfos(string line, int lineNumber)
        {
            LogBuilder builder = new LogBuilder();

            Match match = Regex.Match(line, Pattern);

            if (Regex.IsMatch(line, Pattern))
            {
                builder.Reset();
                builder.BuildDate(match.Groups[LogInfo.KEY_DATE].ToString())
                        .BuildTime(match.Groups[LogInfo.KEY_TIME].ToString())
                        .BuildDateTime()
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
