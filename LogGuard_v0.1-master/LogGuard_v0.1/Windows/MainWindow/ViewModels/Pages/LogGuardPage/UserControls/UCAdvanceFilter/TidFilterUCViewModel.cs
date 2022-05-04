using LogGuard_v0._1.Base.LogGuardFlow.SourceFilter;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.LogWatcher;
using System;
using System.Collections.Generic;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages.LogGuardPage.UserControls.UCAdvanceFilter
{
    public class TidFilterUCViewModel : ChildOfAdvanceFilterUCViewModel
    {
        private List<MatchedWord> matchedWords;

        public TidFilterUCViewModel(BaseViewModel parent) : base(parent)
        {
            matchedWords = new List<MatchedWord>();
        }
        public override bool IsUseFilterEngine => false;

        public override bool Filter(object obj)
        {
            matchedWords.Clear();
            if (string.IsNullOrEmpty(FilterContent))
            {
                return true;
            }

            var data = obj as LWI_ParseableViewModel;
            data.HighlightTidSource = null;

            if (IsFilterEnable && data?.Tid != null)
            {
                var contain = data
                    .Tid
                    .ToString()
                    .IndexOf(FilterContent, StringComparison.InvariantCultureIgnoreCase);
                if(contain != -1)
                {
                    matchedWords.Add(new MatchedWord(contain, FilterContent, data.Tid.ToString()));
                }
                data.HighlightTidSource = matchedWords.ToArray();
                return contain != -1;
            }

            return true;
        }

    }
}
