using LogGuard_v0._1.Base.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages
{
    public class MSW_BasePageViewModel : BaseViewModel, IPageViewModel
    {
        public MSW_BasePageViewModel()
        {
        }

        public MSW_BasePageViewModel(BaseViewModel parentVM) : base(parentVM)
        {
        }

        public virtual void OnLoaded()
        {
            foreach (var child in ChildModels)
            {
                child.OnBegin();
            }
        }

        public virtual bool OnUnloaded()
        {
            foreach (var child in ChildModels)
            {
                child.OnDestroy();
            }
            return true;
        }

    }
}
