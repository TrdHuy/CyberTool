using honeyboard_release_service.@base.module;
using honeyboard_release_service.definitions;
using honeyboard_release_service.extensions;
using honeyboard_release_service.implement.module;
using honeyboard_release_service.implement.view_helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace honeyboard_release_service.implement.view_manager.notebook_header
{
    internal class CalendarNotebookHeaderViewManager : NotebookViewManager
    {

        private Grid? _gridContainer;
        private ListView? _notebookListView;

        private CalendarNotebookHeaderViewManager() : base()
        {
        }

        public static CalendarNotebookHeaderViewManager Current
        {
            get
            {
                return PublisherModuleManager.CNHVM_Instance;
            }
        }

        public override void OnViewInstantiated()
        {
            _gridContainer = PublisherViewHelper
               .Current
               .GetViewByKey(PublisherViewKeyDefinition.CalendarNoteBookGridContainer) as Grid;
            _notebookListView = PublisherViewHelper
                .Current
                .GetViewByKey(PublisherViewKeyDefinition.CalendarNoteBookListView) as ListView;
            if (_gridContainer == null)
            {
                throw new InvalidOperationException("CalendarNoteBookGridContainer not found");
            }

            if (_notebookListView == null)
            {
                throw new InvalidOperationException("CalendarNoteBookListView not found");
            }
        }

        protected override void UpdateCalendarNotebookView()
        {
            if (_gridContainer != null && _notebookListView != null)
            {
                _gridContainer.Children.Clear();
                GenerateGridViewColumn(_gridContainer, ColumnNumber);
                GenerateColumnHeader(_gridContainer, ColumnNumber);
            }
        }

        private void GenerateGridViewColumn(Grid g, int columnsNum)
        {
            // Header view generator
            {
                g.ColumnDefinitions.Clear();
                g.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(_projectColumnWidth, GridUnitType.Pixel)
                });
                for (int i = 0; i < columnsNum; i++)
                {
                    g.ColumnDefinitions.Add(new ColumnDefinition()
                    {
                        Width = new GridLength(1, GridUnitType.Star)
                    });
                }
                g.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(_rightGapColumnWidth, GridUnitType.Pixel)
                });
            }

        }

        private void GenerateColumnHeader(Grid g, int columnsNum)
        {
            for (int i = 1; i <= columnsNum; i++)
            {
                Border headerBorder = new Border()
                {
                    CornerRadius = new CornerRadius(_headerCornerRadius),
                    Margin = new Thickness(_headerMarginLeft
                        , _headerMarginTop
                        , _headerMarginRight
                        , _headerMarginBottom),
                };
                headerBorder.SetResourceReference(Border.BackgroundProperty, "Background_Level3");
                headerBorder.SetValue(Grid.RowProperty, 0);
                headerBorder.SetValue(Grid.ColumnProperty, i);

                TextBlock headerContent = new TextBlock()
                {
                    Text = _startDateTime.AddDays(i - 1).ToString("dddd dd"),
                    Margin = new Thickness(_headerContentMarginLeft, 0, 0, 0),
                    VerticalAlignment = VerticalAlignment.Center,
                    FontWeight = FontWeights.Bold,
                    FontSize = _headerContentFontSize,
                };

                headerContent.SetResourceReference(TextBlock.ForegroundProperty, "Foreground_Level3");
                headerBorder.Child = headerContent;
                g.Children.Add(headerBorder);
            }
        }

    
    }
}
