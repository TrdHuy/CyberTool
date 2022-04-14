using LogGuard_v0._1.Windows.MainWindow.Models;
using LogGuard_v0._1.Windows.MainWindow.Models.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Base.AndroidLog.LogParser
{
    public abstract class AbstractLogParser
    {
        protected string DateGroup = @"(?<Date>[0-1]\d-[0-3]\d)" + @"\s+";
        protected string TimeGroup = @"(?<Time>[0-2]\d:[0-6]\d:[0-6]\d.\d+)" + @"\s+";
        protected string PidGroup = @"(?<Pid>\w+)" + @"\s+";
        protected string TidGroup = @"(?<Tid>\w+)" + @"\s+";
        protected string PackageGroup = @"(?<Package>\S+)" + @"\s";
        protected string LevelGroup = @"(?<Level>[VDIWEF])" + @"\s+";
        protected string TagGroup = @"(?<Tag>\S+)" + @"\s+";
        protected string MessageGroup = @"(?<Message>.*)";
        protected string UidGroup = @"(?<Uid>\w+)" + @"\s+";
        protected string Pattern = "";
        protected string Pattern2 = "";

        public AbstractLogParser()
        {
            InitDataGroupParser();
        }

        protected virtual void InitDataGroupParser()
        {

        }

        public virtual LogInfo ParseLogInfos(string line, int lineNumber)
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
            }

            return builder.Build();
        }

        public virtual bool IsMatch(string line)
        {
            bool result = false;
            result = Regex.IsMatch(line, Pattern);
            if (Pattern2.Length > 0 && !result)
                result = Regex.IsMatch(line, Pattern2);
            return result;
        }
    }
}
