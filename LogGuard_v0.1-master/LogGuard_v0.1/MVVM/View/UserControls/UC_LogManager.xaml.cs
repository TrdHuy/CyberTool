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

            //MenuItem root = new MenuItem(Items) { Title = "Menu" };
            //MenuItem childItem1 = new MenuItem(root) { Title = "Child item #1" };
            //root.Items.Add(childItem1);
            //root.Items.Add(new MenuItem(root) { Title = "Child item #2" });

            //Items.Add(root);
            //TeamTreeView.ItemsSource = Items;

            //for (int i = 0; i < 3; i++)
            //{
            //    MenuItem root2 = new MenuItem(Items) { Title = i + "" };
            //    Items.Add(root2);

            //}

            //for (int i = 0; i < 3; i++)
            //{
            //    MenuItem root2 = new MenuItem(childItem1) { Title = i + "" };
            //    childItem1.Items.Add(root2);

            //}
        }


    }

    public class MenuItem : IHanzaTreeViewItem
    {
        private BaseCommandImpl addCmd;
        private BaseCommandImpl rmCmd;
        private object parent;
        public MenuItem(object par)
        {
            this.Items = new ObservableCollection<MenuItem>();
            this.parent = par;
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
                    if (parent is MenuItem)
                    {
                        var cast = parent as MenuItem;
                        cast?.Items.Remove(hzItem.DataContext as MenuItem);
                    }
                    else if (parent is ObservableCollection<MenuItem>)
                    {
                        var cast = parent as ObservableCollection<MenuItem>;
                        cast?.Remove(hzItem.DataContext as MenuItem);
                    }
                }
            });
        }
        public string Title { get; set; }
        public ObservableCollection<MenuItem> Items { get; set; }
        public ICommand AddBtnCommand { get => addCmd; }
        public ICommand RemoveBtnCommand { get => rmCmd; }
    }
}
