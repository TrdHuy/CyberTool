﻿using LogGuard_v0._1.Windows.MainWindow.Models.Builder;
using System.Text.RegularExpressions;

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

        public abstract LogInfo ParseLogInfos(string line, int lineNumber);
       
    }
}