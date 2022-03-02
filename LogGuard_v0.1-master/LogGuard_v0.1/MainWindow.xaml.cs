using LogGuard_v0._1.LogGuard.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace LogGuard_v0._1
{
    public class ViewModel : INotifyPropertyChanged, ILogWatcherElements
    {
        private static Random random = new Random();

        public event PropertyChangedEventHandler PropertyChanged;

        public ViewModel()
        {
            AutoGenerateContent();
        }
        public void OnChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public int Line { get; set; }
        public string Level { get; set; }
        public string Content1 { get; set; }
        public string Content2 { get; set; }
        public string Content3 { get; set; }

        public void AutoGenerateContent()
        {
            Content1 = RandomString(random.Next(20, 1000));
            Content2 = RandomString(random.Next(10, 15));
            Content3 = RandomString(random.Next(8, 15));
            Level = RandomString(1, "DVEFIW");
        }

        public static string RandomString(int length, string chars = "")
        {
            chars = string.IsNullOrEmpty(chars) ? "ABCDE FGHIJKLMNOPQRSTUVWXYZqwertyuiopasdfghjklmnbvcxz0123456789" : chars;
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }

    public class RangeObservableCollection<T> : ObservableCollection<T>
    {
        private object syncObject = new object();
        public RangeObservableCollection()
        {
            BindingOperations.EnableCollectionSynchronization(this, syncObject);
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public RangeObservableCollection<ViewModel> RawSource { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            int quantity = 100000;
            RawSource = new RangeObservableCollection<ViewModel>();
            for (int i = 0; i < quantity; i++)
            {
                var x = new ViewModel() { Line = i };
                RawSource.Add(x);
            }
            LogView.LogWatcherItemsSource = RawSource;

        }
    }
}
