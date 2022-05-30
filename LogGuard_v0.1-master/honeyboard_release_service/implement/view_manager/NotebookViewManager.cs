using honeyboard_release_service.definitions;
using honeyboard_release_service.extensions;
using honeyboard_release_service.implement.module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace honeyboard_release_service.implement.view_manager
{
    internal abstract class NotebookViewManager : BasePublisherModule
    {
        protected float _projectColumnWidth = CalendarNotebookSizeDefinition.ProjectColumnWidth;
        protected float _rightGapColumnWidth = CalendarNotebookSizeDefinition.RightGapColumnWidth;
        protected float _headerCornerRadius = CalendarNotebookSizeDefinition.HeaderCornerRadius;
        protected float _headerMarginLeft = CalendarNotebookSizeDefinition.HeaderMarginLeft;
        protected float _headerMarginTop = CalendarNotebookSizeDefinition.HeaderMarginTop;
        protected float _headerMarginRight = CalendarNotebookSizeDefinition.HeaderMarginRight;
        protected float _headerMarginBottom = CalendarNotebookSizeDefinition.HeaderMarginBottom;
        protected float _headerContentMarginLeft = CalendarNotebookSizeDefinition.HeaderContentMarginLeft;
        protected float _headerContentFontSize = CalendarNotebookSizeDefinition.HeaderContentFontSize;
        protected float _notebookItemHeight = CalendarNotebookSizeDefinition.NotebookItemHeight;
        protected float _headerItemHeight = CalendarNotebookSizeDefinition.HeaderItemHeight;
        protected float _headerItemWidth = CalendarNotebookSizeDefinition.HeaderItemWidth;
        protected float _headerItemCornerRad = CalendarNotebookSizeDefinition.HeaderItemCornerRad;
        protected float _headerItemFontSize = CalendarNotebookSizeDefinition.HeaderItemFontSize;
        protected float _itemCornerRad = CalendarNotebookSizeDefinition.ItemCornerRad;
        protected float _itemMargin = CalendarNotebookSizeDefinition.ItemMargin;
        protected float _itemFontSize = CalendarNotebookSizeDefinition.ItemFontSize;

        protected DateTime _endDateTime;
        protected DateTime _startDateTime;
        public int ColumnNumber { get; private set; }

        protected NotebookViewManager()
        {
            _startDateTime = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
            _endDateTime = DateTime.Now.EndOfWeek(DayOfWeek.Sunday);
        }

        public void UpdateColumnNumber(DateTime start, DateTime end)
        {
            _startDateTime = start;
            _endDateTime = end;
            ColumnNumber = Convert.ToInt32((_endDateTime - _startDateTime).TotalDays) + 1;
            UpdateCalendarNotebookView();
        }

        protected abstract void UpdateCalendarNotebookView();
    }
}
