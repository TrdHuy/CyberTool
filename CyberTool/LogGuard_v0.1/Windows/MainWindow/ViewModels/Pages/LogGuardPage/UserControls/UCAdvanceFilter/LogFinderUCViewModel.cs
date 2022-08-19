using LogGuard_v0._1.AppResources.AttachedProperties;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.LogGuardFlow.SourceHighlightManager;
using LogGuard_v0._1.Implement.UIEventHandler;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.LogWatcher;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages.LogGuardPage.UserControls.UCAdvanceFilter
{
    public class LogFinderUCViewModel : ChildOfAdvanceFilterUCViewModel
    {
        public override bool IsUseFilterEngine { get => true; }

        public LogFinderUCViewModel(BaseViewModel parent) : base(parent)
        {
            FilterLeftClickCommand = new CommandExecuterModel((paramaters) =>
            {
                switch (CurrentFilterMode)
                {
                    case FilterType.Simple:
                        CurrentFilterMode = FilterType.Syntax;
                        break;
                    case FilterType.Syntax:
                        CurrentFilterMode = FilterType.Advance;
                        break;
                    case FilterType.Advance:
                        CurrentFilterMode = FilterType.Simple;
                        break;
                }
                return null;
            });
        }

        public override bool Filter(object obj)
        {
            if (!CurrentEngine.IsVaild())
            {
                CurrentEngine.Refresh();
            }
            return true;
        }

        protected override bool DoHighlight(object obj)
        {
            var data = obj as LWI_ParseableViewModel;

            if (data != null)
            {
                if (FilterContent == "")
                {
                    data.ExtraHighlightMessageSource = null;
                    data.ExtraHighlightTagSource = null;
                    return false;
                }

                if (data.Message.Equals(""))
                {
                    return false;
                }

                CurrentEngine.ContainIgnoreCase(data.Message.ToString());

                data.ExtraHighlightMessageSource = CurrentEngine
                           .GetMatchWords()
                           .OrderBy(o => o.StartIndex)
                           .ToArray();

                CurrentEngine.ContainIgnoreCase(data.Tag.ToString());

                data.ExtraHighlightTagSource = CurrentEngine
                           .GetMatchWords()
                           .OrderBy(o => o.StartIndex)
                           .ToArray();

                return !CurrentEngine.IsMatchLstEmpty;
            }
            return false;
        }

        protected override void DoCleanHighlightSource(object obj)
        {
            var data = obj as LWI_ParseableViewModel;
            if (data != null)
            {
                data.ExtraHighlightMessageSource = null;
                data.ExtraHighlightTagSource = null;
            }
        }

        protected override void OnComparableSourceUpdated(object sender, object args)
        {
            SourceHighlightManagerImpl.Current.NotifyHighlightPropertyChanged(this, args);
        }

    }
}

