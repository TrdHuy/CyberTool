using honeyboard_release_service.definitions;
using honeyboard_release_service.implement.module;
using honeyboard_release_service.implement.view_helper;
using honeyboard_release_service.implement.view_manager.notebook_header;
using honeyboard_release_service.models.VOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace honeyboard_release_service.implement.view_manager.notebook_item
{
    internal class CalendarNotebookItemViewManager : NotebookViewManager
    {
        private ListView? _notebookListView;
        private Grid? _itemGridContainer;


        public static CalendarNotebookItemViewManager Current
        {
            get
            {
                return PublisherModuleManager.CNIVM_Instance;
            }
        }

        private CalendarNotebookItemViewManager()
        {
        }

        public override void OnViewInstantiated()
        {
            _notebookListView = PublisherViewHelper
                .Current
                .GetViewByKey(PublisherViewKeyDefinition.CalendarNoteBookListView) as ListView;
            if (_notebookListView == null)
            {
                throw new InvalidOperationException("CalendarNoteBookListView not found");
            }
        }

        public void UpdateNotebookItemContent(ProjectVO project)
        {
            if (_itemGridContainer != null
                && !string.IsNullOrEmpty(project.OnBranch?.BranchPath)
                && (project.VersionMap?.ContainsKey(project.OnBranch.BranchPath) ?? false))
            {
                var branchVersionMap = project.VersionMap[project.OnBranch.BranchPath];
                for (int i = 0; i < ColumnNumber; i++)
                {
                    var currentDate = _startDateTime.Date.AddDays(i);

                    if (project.VersionMap != null && branchVersionMap.ContainsKey(currentDate))
                    {
                        var source = branchVersionMap[currentDate];

                        Border itemBorder = new Border()
                        {
                            CornerRadius = new CornerRadius(_itemCornerRad),
                            Margin = new Thickness(_itemMargin)
                        };
                        itemBorder.SetValue(Grid.RowProperty, 1);
                        itemBorder.SetValue(Grid.ColumnProperty, i + 1);
                        itemBorder.SetResourceReference(Border.BackgroundProperty, "Background_Level4");

                        var listV = new ListView()
                        {
                            ItemsSource = source,
                            FontSize = _itemFontSize,
                        };
                        listV.SetResourceReference(ListView.StyleProperty, "NotebookDayVersionPresenterStyle");
                        listV.SetResourceReference(ListView.ForegroundProperty, "Foreground_Level3");
                        listV.SetResourceReference(ListView.BackgroundProperty, "Background_Level4");
                        itemBorder.Child = listV;
                        _itemGridContainer.Children.Add(itemBorder);
                    }
                }

            }

        }

        protected override void UpdateCalendarNotebookView()
        {
            _itemGridContainer = new Grid();
            if (_itemGridContainer != null && _notebookListView != null)
            {
                _itemGridContainer.Children.Clear();
                GenerateGridViewColumn(_itemGridContainer, ColumnNumber);
                GenerateGridViewContent(_itemGridContainer, ColumnNumber);
            }
        }

        private void GenerateGridViewContent(Grid g, int columnNumber)
        {
            g.RowDefinitions.Clear();
            g.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Pixel) });
            g.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

            // Hor line
            {
                Rectangle horLine = new Rectangle()
                {
                    Height = 1,

                };
                horLine.SetResourceReference(Rectangle.FillProperty, "Foreground_Level2");
                horLine.SetValue(Grid.RowProperty, 0);
                horLine.SetValue(Grid.ColumnSpanProperty, columnNumber + 2);

                g.Children.Add(horLine);
            }

            // Header content
            {
                Border headerBorder = new Border()
                {
                    Width = _headerItemWidth,
                    Height = _headerItemHeight,
                    CornerRadius = new CornerRadius(_headerItemCornerRad)
                };
                headerBorder.SetResourceReference(Border.BackgroundProperty, "ButtonBackground_Level1");
                headerBorder.SetValue(Grid.RowProperty, 1);
                TextBlock headerContent = new TextBlock()
                {
                    FontSize = _headerItemFontSize,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    RenderTransformOrigin = new Point(0.5, 0.5),
                    LayoutTransform = new RotateTransform(270),
                };
                headerContent.SetResourceReference(TextBlock.ForegroundProperty, "Foreground_Level3");
                Binding textBinding = new Binding("DisplayName") { FallbackValue = "HoneyBoard" };
                headerContent.SetBinding(TextBlock.TextProperty, textBinding);
                headerBorder.Child = headerContent;

                g.Children.Add(headerBorder);
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


        public object? GetItemContent()
        {
            return _itemGridContainer;
        }


    }
}
