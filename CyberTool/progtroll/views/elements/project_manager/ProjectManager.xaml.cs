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
        private CollectionView _viewCache;

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

            _viewCache = (CollectionView)CollectionViewSource.GetDefaultView(PART_VersionHistoryListView.ItemsSource);
            _viewCache.Filter += DoFilter;
        }

        private void OnCommitTaskIdSourceChanged(List<string>? newSource)
        {
            if (!IsInitialized) return;
            ClearOldMenuItem();
            _filterTaskIdList = newSource;

            if (newSource != null)
            {
                PART_NoneTaskIdFilter.IsEnabled = newSource.Contains("");

                foreach (var item in newSource)
                {
                    if (item != "")
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
                        };

                        menuItem.Checked += (s, e) =>
                        {
                            _filterTaskIdList?.Add(item);
                        };

                        menuItem.Click += (s, e) =>
                        {
                            _viewCache.Refresh();
                        };
                        PART_TaskIdItem.Items.Add(menuItem);
                    }
                }
            }
        }

        private void ClearOldMenuItem()
        {
            for (int i = PART_TaskIdItem.Items.Count - 1; i > 2; i--)
            {
                PART_TaskIdItem.Items.RemoveAt(i);
            }
        }

        private bool DoFilter(object item)
        {
            var versionHistoryItem = item as VersionHistoryItemViewModel;
            if (_filterTaskIdList != null
                && versionHistoryItem != null
                && _filterTaskIdList.Count > 0)
            {
                return (_filterTaskIdList.Contains(versionHistoryItem.VersionCommitVO.TaskId));
            }
            return true;
        }

        private void HandleButtonAndMenuItemClick(object sender, RoutedEventArgs e)
        {
            if (PART_TaskIdItem.Items.Count > 2)
            {
                PART_TaskIdItem.IsOpen = true;
            }
        }

        private void HandleCheckedEvent(object sender, RoutedEventArgs e)
        {
            if (!IsInitialized) return;

            var menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                switch (menuItem.Name)
                {
                    case "PART_AllFilterTaskId":
                        {
                            foreach (var item in PART_TaskIdItem.Items)
                            {
                                if (item != PART_AllFilterTaskId
                                    && item != PART_SeparatorTaskIdList
                                    && item != PART_NoneTaskIdFilter)
                                {
                                    MenuItem? submenuItem = item as MenuItem;
                                    if (submenuItem != null)
                                    {
                                        submenuItem.IsChecked = true;
                                    }
                                }
                            }
                            _viewCache?.Refresh();
                            break;
                        }
                    case "PART_NoneTaskIdFilter":
                        {
                            _filterTaskIdList?.Add("");
                            _viewCache?.Refresh();
                            break;
                        }
                }
            }

        }

        private void HandleUncheckedEvent(object sender, RoutedEventArgs e)
        {
            if (!IsInitialized) return;

            var menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                switch (menuItem.Name)
                {
                    case "PART_AllFilterTaskId":
                        {
                            foreach (var item in PART_TaskIdItem.Items)
                            {
                                if (item != PART_AllFilterTaskId
                                    && item != PART_SeparatorTaskIdList
                                    && item != PART_NoneTaskIdFilter)
                                {
                                    MenuItem? subMenuItem = item as MenuItem;
                                    if (subMenuItem != null)
                                    {
                                        subMenuItem.IsChecked = false;
                                    }
                                }
                            }
                            _viewCache?.Refresh();
                            break;
                        }
                    case "PART_NoneTaskIdFilter":
                        {
                            _filterTaskIdList?.Remove("");
                            _viewCache?.Refresh();
                            break;
                        }
                }
            }
        }
    }
}
