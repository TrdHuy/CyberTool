using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Input;
using cyber_base.view_model;
using log_guard.views.others.log_watcher._item;
using cyber_base.implement.command;

namespace log_guard.view_models.log_manager.by_team
{
    public class LogByTeamItemViewModel : BaseViewModel, IHanzaTreeViewItem
    {
        private BaseCommandImpl addCmd;
        private BaseCommandImpl rmCmd;
        private object parent;

        public LogByTeamItemViewModel(LogByTeamItemViewModel par, string title)
        {
            this.Items = new ObservableCollection<LogByTeamItemViewModel>();
            this.parent = par;
            this.Title = title;
            InitCmd();
        }

        public LogByTeamItemViewModel(ObservableCollection<LogByTeamItemViewModel> par, string title)
        {
            this.Items = new ObservableCollection<LogByTeamItemViewModel>();
            this.parent = par;
            this.Title = title;
            InitCmd();
        }

        private void InitCmd()
        {
            addCmd = new BaseCommandImpl((s, e) =>
            {
                int a = 1;
            });

            rmCmd = new BaseCommandImpl((s, e) =>
            {
                int a = 1;
                var hzItem = s as HanzaTreeViewItem;
                if (parent != null && hzItem != null)
                {
                    if (parent is LogByTeamItemViewModel)
                    {
                        var cast = parent as LogByTeamItemViewModel;
                        cast?.Items.Remove(hzItem.DataContext as LogByTeamItemViewModel);
                    }
                    else if (parent is ObservableCollection<LogByTeamItemViewModel>)
                    {
                        var cast = parent as ObservableCollection<LogByTeamItemViewModel>;
                        cast?.Remove(hzItem.DataContext as LogByTeamItemViewModel);
                    }
                }
            });
        }

        public string Title { get; set; }
        public ObservableCollection<LogByTeamItemViewModel> Items { get; set; }
        public BaseCommandImpl AddBtnCommand { get => addCmd; }
        public BaseCommandImpl RemoveBtnCommand { get => rmCmd; }
        
        public void AddItem(LogByTeamItemViewModel item)
        {
            Items.Add(item);
        }

        public void RemoveItem(LogByTeamItemViewModel item)
        {
            Items.Remove(item);
        }
    }
}
