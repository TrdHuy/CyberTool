using LogGuard_v0._1.Base.Command;
using LogGuard_v0._1.LogGuard.Control;
using LogGuard_v0._1.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LogGuard_v0._1.MVVM.View.UserControls
{
    /// <summary>
    /// Interaction logic for UC_LogManager.xaml
    /// </summary>
    public partial class UC_LogManager : UserControl
    {
        public ObservableCollection<MenuItem> Items { get; set; } = new ObservableCollection<MenuItem>();

        public UC_LogManager()
        {
            InitializeComponent();

            var source = new ObservableCollection<LogByTeamItemViewModel>();

            var perForItem = new LogByTeamItemViewModel(source, "P4");

            perForItem.AddItem(new LogByTeamItemViewModel(perForItem, "Setting"));
            perForItem.AddItem(new LogByTeamItemViewModel(perForItem, "Config"));

            for (int i = 0; i < 1000; i++)
            {
                perForItem.AddItem(new LogByTeamItemViewModel(perForItem, "" + i));
            }

            var item1 = new LogByTeamItemViewModel(source, "SIP team");
            item1.AddItem(new LogByTeamItemViewModel(item1, "Performance"));
            item1.AddItem(new LogByTeamItemViewModel(item1, "View"));

            source.Add(item1);
            source.Add(perForItem);
            TeamTreeView.ItemsSource = source;
            lbtHeader.MouseDoubleClickCommand = new BaseCommandImpl((param, e) =>
            {
                App.Current.ShowPopupCControl(lbtCC, lbtHeader, OwnerWindow.MainScreen, 600, 440);
            });

        }


    }
}
