using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace log_guard.views.others.log_watcher._grid_view
{
    public class GridViewWatcher : GridView
    {
        #region ColumnsStyleSource
        public static readonly DependencyProperty ColumnsStyleSourceProperty =
                DependencyProperty.Register(
                        "ColumnsStyleSource",
                        typeof(IEnumerable<Style>),
                        typeof(GridViewWatcher),
                        new PropertyMetadata(
                                default(IEnumerable<Style>),
                                new PropertyChangedCallback(ColumnsStyleSourcePropertyChangedCallback)),
                        null);

        private static void ColumnsStyleSourcePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as GridViewWatcher;
            if (ctrl == null) return;
            ctrl.HeaderStyleSource = e.NewValue as IEnumerable<Style>;
            ctrl.HeaderStyleSourceCount = ctrl.HeaderStyleSource?.Count() == null ? 0 : ctrl.HeaderStyleSource.Count();
            ctrl.RestyleColumnHeader();
        }

        public IEnumerable<Style> ColumnsStyleSource
        {
            get { return (IEnumerable<Style>)GetValue(ColumnsStyleSourceProperty); }
            set { SetValue(ColumnsStyleSourceProperty, value); }
        }
        #endregion

        private IEnumerable<Style> HeaderStyleSource { get; set; }
        private int HeaderStyleSourceCount { get; set; }

        public GridViewWatcher()
        {

            var columnsNotifier = Columns as INotifyCollectionChanged;
            if (columnsNotifier != null)
            {
                columnsNotifier.CollectionChanged += HandleGridViewWatcherColumnsCollectionChanged;
            }

        }

        private void HandleGridViewWatcherColumnsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if(HeaderStyleSourceCount == 0)
                {
                    return;
                }

                int i = 0;
                foreach (var item in e.NewItems)
                {
                    var columnHeader = (item as GridViewColumn)?.Header as GridViewColumnHeader;

                    if (columnHeader != null)
                    {
                        columnHeader.Style = HeaderStyleSource.ElementAt((e.NewStartingIndex + i) % HeaderStyleSourceCount);
                        columnHeader.SizeChanged += HandleColumnHeaderSizeChanged;
                    }
                    i++;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (var item in e.NewItems)
                {
                    var columnHeader = (item as GridViewColumn)?.Header as GridViewColumnHeader;

                    if (columnHeader != null)
                    {
                        columnHeader.SizeChanged -= HandleColumnHeaderSizeChanged;
                    }
                }
            }
        }

        private void HandleColumnHeaderSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width == 0)
            {
                RestyleColumnHeader();
            }
            else if (e.PreviousSize.Width == 0 && e.NewSize.Width != 0)
            {
                RestyleColumnHeader();
            }
        }

        private void RestyleColumnHeader()
        {
            int j = 0;
            for (int i = 0; i < Columns.Count; i++)
            {
                var header = Columns[i].Header as GridViewColumnHeader;
                if (header != null && header.ActualWidth != 0)
                {
                    header.Style = HeaderStyleSource.ElementAt(j % HeaderStyleSourceCount);
                    j++;
                }
            }
        }
    }

}
