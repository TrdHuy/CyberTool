using honeyboard_release_service.views.elements.calendar_notebook.@base;
using honeyboard_release_service.views.elements.calendar_notebook.data_structure;
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

namespace honeyboard_release_service.views.elements.calendar_notebook
{
    /// <summary>
    /// Interaction logic for CalendarNotebookListItem.xaml
    /// </summary>
    public partial class CalendarNotebookListItem : UserControl
    {
        public const int RUNE_STRETCH_COLUMNS = 2;
        private int _currentColumnNuber = 0;

        #region ItemsSource
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                "ItemsSource",
                typeof(NotebookListObservableCollection<ObservableCollection<ICalendarNotebookCommitItemContext>>),
                typeof(CalendarNotebookListItem),
                new PropertyMetadata(default(NotebookListObservableCollection<ObservableCollection<ICalendarNotebookCommitItemContext>>)
                    , new PropertyChangedCallback(OnItemsSourceChangedCallback)));

        private static void OnItemsSourceChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as CalendarNotebookListItem;
            ctrl?.UpdateItemViewWhenSourceChanged(
                e.OldValue as NotebookListObservableCollection
                    <ObservableCollection<ICalendarNotebookCommitItemContext>>,
                e.NewValue as NotebookListObservableCollection
                    <ObservableCollection<ICalendarNotebookCommitItemContext>>);
        }

        public NotebookListObservableCollection<ObservableCollection<ICalendarNotebookCommitItemContext>> ItemsSource
        {
            get { return (NotebookListObservableCollection<ObservableCollection<ICalendarNotebookCommitItemContext>>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        #endregion

        #region ItemCornerRad
        public static readonly DependencyProperty ItemCornerRadProperty =
            DependencyProperty.Register(
                "ItemCornerRad",
                typeof(double),
                typeof(CalendarNotebookListItem),
                new PropertyMetadata(5d));

        public double ItemCornerRad
        {
            get { return (double)GetValue(ItemCornerRadProperty); }
            set { SetValue(ItemCornerRadProperty, value); }
        }
        #endregion

        #region ItemMargin
        public static readonly DependencyProperty ItemMarginProperty =
            DependencyProperty.Register(
                "ItemMargin",
                typeof(double),
                typeof(CalendarNotebookListItem),
                new PropertyMetadata(5d));

        public double ItemMargin
        {
            get { return (double)GetValue(ItemMarginProperty); }
            set { SetValue(ItemMarginProperty, value); }
        }
        #endregion

        #region ItemFontSize
        public static readonly DependencyProperty ItemFontSizeProperty =
            DependencyProperty.Register(
                "ItemFontSize",
                typeof(double),
                typeof(CalendarNotebookListItem),
                new PropertyMetadata(10d));

        public double ItemFontSize
        {
            get { return (double)GetValue(ItemFontSizeProperty); }
            set { SetValue(ItemFontSizeProperty, value); }
        }
        #endregion

        public CalendarNotebookListItem()
        {
            InitializeComponent();
        }

        private void UpdateItemViewWhenSourceChanged(
            NotebookListObservableCollection
                <ObservableCollection<ICalendarNotebookCommitItemContext>>? oldItemsSource,
            NotebookListObservableCollection
                <ObservableCollection<ICalendarNotebookCommitItemContext>>? newItemsSource)
        {
            if (oldItemsSource != null)
            {
                oldItemsSource.SizeChanged -= HandleItemsSourceSizeChanged;
            }
            if (newItemsSource != null)
            {
                if (_currentColumnNuber != newItemsSource.Size - RUNE_STRETCH_COLUMNS)
                {
                    _currentColumnNuber = newItemsSource.Size - RUNE_STRETCH_COLUMNS;
                    FramingGridViewColumn(PART_MainContainerGrid, _currentColumnNuber);
                    GenerateNotebookItemContentForListViewType(PART_MainContainerGrid
                        , _currentColumnNuber
                        , ItemMargin
                        , ItemFontSize
                        , RUNE_STRETCH_COLUMNS
                        , this);

                    newItemsSource.SizeChanged -= HandleItemsSourceSizeChanged;
                    newItemsSource.SizeChanged += HandleItemsSourceSizeChanged;
                }

            }
        }

        private void HandleItemsSourceSizeChanged(object sender, int oldSize, int newSize)
        {
            if (_currentColumnNuber != newSize - RUNE_STRETCH_COLUMNS)
            {
                _currentColumnNuber = newSize - RUNE_STRETCH_COLUMNS;

                ExtendOrShrinkGridViewColumn(PART_MainContainerGrid, newSize - oldSize);
                ExtendOrShrinkNotebookItemContentForListViewType(PART_MainContainerGrid
                    , newSize - oldSize
                    , ItemMargin
                    , ItemFontSize
                    , RUNE_STRETCH_COLUMNS
                    , this);
            }
        }

        private void ExtendOrShrinkNotebookItemContentForListViewType(Grid g
            , int columnsNum
            , double itemMargin
            , double itemFontSize
            , int stretchColumn
            , object context)
        {
            var count = Math.Abs(columnsNum);
            if (columnsNum < 0 && g.ColumnDefinitions.Count < count)
            {
                g.Children.Clear();
                return;
            }

            int columnIndexGap = (stretchColumn + 1) / 2;

            for (int i = 0; i < count; i++)
            {
                if (columnsNum > 0)
                {
                    var listV = GetSubItemView(itemMargin
                       , itemFontSize
                       , g.Children.Count + columnIndexGap + i
                       , g.Children.Count
                       , context);
                    g.Children.Add(listV);
                }
                else
                {
                    var view = g.Children[g.Children.Count - 1] as ListView;
                    BindingOperations.ClearBinding(view, ListView.ItemsSourceProperty);
                    g.Children.RemoveAt(g.Children.Count - 1);
                }
            }
        }

        private static void ExtendOrShrinkGridViewColumn(Grid g
            , int columnsNum)
        {
            var count = Math.Abs(columnsNum);
            if (columnsNum < 0 && g.ColumnDefinitions.Count < count)
            {
                g.ColumnDefinitions.Clear();
                return;
            }

            for (int i = 0; i < count; i++)
            {
                if (columnsNum > 0)
                {
                    g.ColumnDefinitions.Add(new ColumnDefinition()
                    {
                        Width = new GridLength(1, GridUnitType.Star)
                    });
                }
                else
                {
                    g.ColumnDefinitions.RemoveAt(g.ColumnDefinitions.Count - 1);
                }
            }

        }

        private static void FramingGridViewColumn(Grid g
            , int columnsNum)
        {
            g.ColumnDefinitions.Clear();
            for (int i = 0; i < columnsNum; i++)
            {
                g.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });
            }
        }

        private static void GenerateNotebookItemContentForListViewType(Grid contentPanel
            , int columnNumber
            , double itemMargin
            , double itemFontSize
            , int strechColumn
            , object context)
        {
            if (contentPanel != null)
            {
                contentPanel.Children.Clear();
                int columnIndexGap = (strechColumn + 1) / 2;
                for (int i = 0; i < columnNumber; i++)
                {

                    var listV = GetSubItemView(itemMargin
                        , itemFontSize
                        , i + columnIndexGap
                        , i
                        , context);
                    contentPanel.Children.Add(listV);
                }
            }
        }

        private static UIElement GetSubItemView(double itemMargin
            , double itemFontSize
            , int sourceIndex
            , int column
            , object context)
        {
            Binding itemSourceBinding = new Binding();
            itemSourceBinding.Source = context;
            itemSourceBinding.Path = new PropertyPath("ItemsSource[" + sourceIndex + "]");
            itemSourceBinding.Mode = BindingMode.TwoWay;
            itemSourceBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            var listV = new ListView()
            {
                FontSize = itemFontSize,
                Margin = new Thickness(itemMargin),
            };

            listV.SetValue(Grid.RowProperty, 1);
            listV.SetValue(Grid.ColumnProperty, column);
            listV.SetResourceReference(ListView.StyleProperty, "NotebookDayVersionPresenterStyle");
            listV.SetResourceReference(ListView.ForegroundProperty, "Foreground_Level3");
            listV.SetBinding(ListView.ItemsSourceProperty, itemSourceBinding);
            return listV;
        }
    }
}
