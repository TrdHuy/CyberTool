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
        public UC_LogManager()
        {
            InitializeComponent();

            var source = new ObservableCollection<LogByTeamItemViewModel>();

            var perForItem = new LogByTeamItemViewModel("P4");
            perForItem.AddItem(new LogByTeamItemViewModel("Setting"));
            perForItem.AddItem(new LogByTeamItemViewModel("Config1"));
           

            var item1 = new LogByTeamItemViewModel("SIP team")
                .AddItem(perForItem)
                .AddItem(new LogByTeamItemViewModel("Performance"))
                .AddItem(new LogByTeamItemViewModel("View"));

            source.Add(item1);
            source.Add(item1);
            source.Add(item1);
            source.Add(item1);
            TeamTreeView.ItemsSource = source;
        }
    }
}
