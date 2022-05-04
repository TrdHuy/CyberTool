using LogGuard_v0._1.Base.LogGuardFlow;
using LogGuard_v0._1.Base.LogGuardFlow.SourceFilter;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.UIEventHandler;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.LogWatcher;
using System;
using System.Collections.Generic;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages.LogGuardPage.UserControls.UCAdvanceFilter
{
    public class PidFilterUCViewModel : ChildOfAdvanceFilterUCViewModel
    {
        private List<MatchedWord> matchedWords;

        public override bool IsUseFilterEngine => false;

        public PidFilterUCViewModel(BaseViewModel parent) : base(parent)
        {
            matchedWords = new List<MatchedWord>();
        }

        public override bool Filter(object obj)
        {
            matchedWords.Clear();

            if (string.IsNullOrEmpty(FilterContent))
            {
                return true;
            }

            var data = obj as LWI_ParseableViewModel;
            data.HighlightPidSource = null;

            if (IsFilterEnable && data?.Pid != null)
            {
                var contain = data
                    .Pid
                    .ToString()
                    .IndexOf(FilterContent, StringComparison.InvariantCultureIgnoreCase);
                if (contain != -1)
                {
                    matchedWords.Add(new MatchedWord(contain, FilterContent, data.Pid.ToString()));
                }
                data.HighlightPidSource = matchedWords.ToArray();
                return contain != -1;
            }

            return true;
        }

    }
}
