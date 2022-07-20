using cyber_tool.app_resources.controls.cyber_window;
using cyber_tool.definitions;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace cyber_tool.windows.cyber_imes.views
{
    /// <summary>
    /// Interaction logic for CyberIMesWindow.xaml
    /// </summary>
    public partial class CyberIMesWindow : CyberWindow
    {
        private double _cacheLoadedHeight;
        private double _cacheLoadedWidth;
        public CyberIMesBoxResult MesResult { get; private set; } = CyberIMesBoxResult.None;

        public CyberIMesWindow()
        {
            Init();
        }

        public CyberIMesWindow(string title
            , string pathIcon
            , string content
            , string yesBtnContent
            , string noBtnContent
            , string continueBtnContent
            , string cancelBtnContent
            , Window owner)
        {
            Init();
            if (owner != null)
            {
                Owner = owner;
            }

            Title = title;
            MainContent.Content = content;
            IconPath.Data = Geometry.Parse(pathIcon);
            if (string.IsNullOrEmpty(yesBtnContent))
            {
                YesBtn.Visibility = Visibility.Collapsed;
            }
            else
            {
                YesBtn.Content = yesBtnContent;
            }

            if (string.IsNullOrEmpty(noBtnContent))
            {
                NoBtn.Visibility = Visibility.Collapsed;
            }
            else
            {
                NoBtn.Content = noBtnContent;
            }

            if (string.IsNullOrEmpty(continueBtnContent))
            {
                ContinueBtn.Visibility = Visibility.Collapsed;
            }
            else
            {
                ContinueBtn.Content = continueBtnContent;
            }

            if (string.IsNullOrEmpty(cancelBtnContent))
            {
                cancelBtn.Visibility = Visibility.Collapsed;
            }
            else
            {
                cancelBtn.Content = cancelBtnContent;
            }

            if (Owner != null)
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
        }

        private void Init()
        {
            InitializeComponent();
            Loaded += MessageBox_Loaded;
            YesBtn.Click += (s, e) =>
            {
                MesResult = CyberIMesBoxResult.Yes;
                this.Close();
            };
            NoBtn.Click += (s, e) =>
            {
                MesResult = CyberIMesBoxResult.No;
                this.Close();
            };
            ContinueBtn.Click += (s, e) =>
            {
                MesResult = CyberIMesBoxResult.Continue;
                this.Close();
            };
            cancelBtn.Click += (s, e) =>
            {
                MesResult = CyberIMesBoxResult.Cancel;
                this.Close();
            };
        }
        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);
            if (WindowState == WindowState.Normal)
            {
                this.Height = _cacheLoadedHeight;
                this.Width = _cacheLoadedWidth;
            }
        }

        private void MessageBox_Loaded(object sender, RoutedEventArgs e)
        {
            _cacheLoadedHeight = this.ActualHeight;
            _cacheLoadedWidth = this.ActualWidth;
        }

        public new CyberIMesBoxResult ShowDialog()
        {
            base.ShowDialog();
            return MesResult;
        }
    }
}
