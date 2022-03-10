using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.LogGuard.Control;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace LogGuard_v0._1.MVVM.ViewModels
{
    public class LogByTeamItemViewModel : BaseViewModel, IHanzaTreeViewItem
    {
        private ObservableCollection<LogByTeamItemViewModel> _childs = new ObservableCollection<LogByTeamItemViewModel>();
        private string _title;

        [Bindable(true)]
        public String Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value; 
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public IEnumerable Childs
        {
            get
            {
                return _childs;
            }
            set
            {
                _childs = new ObservableCollection<LogByTeamItemViewModel>(value.OfType<LogByTeamItemViewModel>());
                InvalidateOwn();
            }
        }


        public LogByTeamItemViewModel(string tittle = "Default title", HanzaTreeViewItem parent = null)
        {
            Title = tittle;
        }

        public LogByTeamItemViewModel AddItem(LogByTeamItemViewModel vm)
        {
            _childs.Add(vm);
            return this;
        }

        public void Remove(LogByTeamItemViewModel vm)
        {
            _childs.Remove(vm);
        }

        public void Remove(int idx)
        {
            _childs.RemoveAt(idx);
        }

        public void Clear()
        {
            _childs.Clear();
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
