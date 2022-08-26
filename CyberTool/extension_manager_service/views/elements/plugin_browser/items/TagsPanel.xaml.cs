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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace extension_manager_service.views.elements.plugin_browser.items
{


    /// <summary>
    /// Interaction logic for TagsPanel.xaml
    /// </summary>
    public partial class TagsPanel : UserControl
    {
        #region Tags
        public static readonly DependencyProperty TagsProperty =
            DependencyProperty.Register(
                "Tags",
                typeof(string[]),
                typeof(TagsPanel),
                new PropertyMetadata(default(string[]), new PropertyChangedCallback(OnTagsChangedCallback)));

        private static void OnTagsChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as TagsPanel;
            ctrl?.UpdateTagsPanelContent();
        }

        public string[] Tags
        {
            get => (string[])GetValue(TagsProperty);
            set => SetValue(TagsProperty, value);
        }
        #endregion

        public TagsPanel()
        {
            InitializeComponent();
        }

        private void UpdateTagsPanelContent()
        {
            PART_MainPanel.Children.Clear();
            if (Tags != null)
            {
                foreach (var tag in Tags)
                {
                    var tb = new TextBox();
                    tb.Text = "#" + tag;
                    PART_MainPanel.Children?.Add(tb);
                }
            }

        }
    }
}
