using LogGuard_v0._1.Base.AndroidLog;
using LogGuard_v0._1.Base.AndroidLog.LogParser;
using LogGuard_v0._1.Windows.MainWindow.Models.Builder;
using System.Text.RegularExpressions;

namespace LogGuard_v0._1.Implement.AndroidLog.LogParser
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

            }
            else
            {
                builder.Reset();
                builder.BuildDate("-")
                         .BuildTime("-")
                         .BuildDateTime()
                         .BuildColorByLevel("-")
                         .BuildTag("-")
                         .BuildMessage(line);
            }

            return builder.Build();
        }
    }
}
