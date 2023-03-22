using Microsoft.Build.Utilities;

namespace cyber_build_task
{
    public abstract class BaseCyberInstallerPackageBuilderTask : ICyberInstallerPackageBuilderTask
    {
        public TaskLoggingHelper Log { get; }

        public BaseCyberInstallerPackageBuilderTask(TaskLoggingHelper tlogHepler)
        {
            Log = tlogHepler;
        }

        public abstract bool Execute();
    }
}
