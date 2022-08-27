using progtroll.view_models.project_manager.items;
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

namespace progtroll.views.elements.project_manager
{
    /// <summary>
    /// Interaction logic for ProjectManager.xaml
    /// </summary>
    public partial class ProjectManager : UserControl
    {
        private List<string>? _filterTaskIdList;

        #region CommitTaskIdSource 
        public static readonly DependencyProperty CommitTaskIdSourceProperty
            = DependencyProperty.RegisterAttached("CommitTaskIdSource"
                , typeof(List<string>)
                , typeof(ProjectManager)
                , new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnCommitTaskIdSourceChangedCallback)));

        private static void OnCommitTaskIdSourceChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as ProjectManager;
            var newSource = e.NewValue as List<string>;
            ctrl?.OnCommitTaskIdSourceChanged(newSource);
        }

        public static List<string> GetCommitTaskIdSource(UIElement obj)
        {
            return (List<string>)obj.GetValue(CommitTaskIdSourceProperty);
        }

        public static void SetCommitTaskIdSource(UIElement obj, List<string> value)
        {
            obj.SetValue(CommitTaskIdSourceProperty, value);
        }
        #endregion

       
        public ProjectManager()
        {
            InitializeComponent();

            _filterTaskIdList = new List<string>();
        }

        private void OnCommitTaskIdSourceChanged(List<string>? newSource)
        {
            PART_TaskIdItem.Items.Clear();
            _filterTaskIdList = newSource;

            if (newSource != null)
            {
                foreach (var item in newSource)
                {
                    var menuItem = new MenuItem()
                    {
                        Header = item,
                        IsCheckable = true,
                        IsChecked = true,
                        StaysOpenOnClick = true,
                    };

                    menuItem.Unchecked += (s, e) =>
                    {
                        _filterTaskIdList?.Remove(item);
                        PART_VersionHistoryListView.Items.Filter = DoFilter;
                    };

                    menuItem.Checked += (s, e) =>
                    {
                        _filterTaskIdList?.Add(item);
                        PART_VersionHistoryListView.Items.Filter = DoFilter;
                    };
                    PART_TaskIdItem.Items.Add(menuItem);
                }
            }
        }

        private bool DoFilter (object item)
        {
            var versionHistoryItem = item as VersionHistoryItemViewModel;
            if (_filterTaskIdList != null && versionHistoryItem != null)
            {
                return (_filterTaskIdList.Contains(versionHistoryItem.VersionCommitVO.TaskId));
            }
            return true;
        }

        private void HandleButtonAndMenuItemClick(object sender, RoutedEventArgs e)
        {
            if (PART_TaskIdItem.Items.Count != 0)
            {
                PART_TaskIdItem.IsOpen = true;
            }
        }
    }
}
