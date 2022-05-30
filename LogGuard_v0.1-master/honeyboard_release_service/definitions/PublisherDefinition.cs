﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace honeyboard_release_service.definitions
{
    internal class PublisherDefinition
    {
        public const string SERVICE_PAGE_URI_ORIGINAL_STRING = "pack://application:,,,/honeyboard_release_service;component/views/usercontrols/ServiceView.xaml";
        public static readonly long SERVICE_PAGE_LOADING_DELAY_TIME = 500;
        public const string SERVICE_PAGE_HEADER_GEOMETRY_DATA = "";
    }

    internal struct CalendarNotebookSizeDefinition
    {
        public static float ProjectColumnWidth { get; } = 50;
        public static float RightGapColumnWidth { get; } = 25;
        public static float HeaderCornerRadius { get; } = 5;
        public static float HeaderMarginLeft { get; } = 5;
        public static float HeaderMarginTop { get; } = 10;
        public static float HeaderMarginRight { get; } = 5;
        public static float HeaderMarginBottom { get; } = 5;
        public static float HeaderContentMarginLeft { get; } = 8;
        public static float HeaderContentFontSize { get; } = 10;
        public static float NotebookItemHeight { get; } = 100;
        public static float HeaderItemHeight { get; } = 90;
        public static float HeaderItemWidth { get; } = 30;
        public static float HeaderItemCornerRad { get; } = 5; 
        public static float HeaderItemFontSize { get; } = 10;
        public static float ItemCornerRad { get; } = 5;
        public static float ItemMargin { get; } = 5;
        public static float ItemFontSize { get; } = 10;

    }

    internal enum PublisherViewKeyDefinition
    {
        CalendarNoteBookBorder = 1,
        CalendarNoteBookGridContainer = 2,
        CalendarNoteBookListView = 3,
    }

    internal enum PublisherCalendarNotebookViewType
    {
        DayOfWeek = 1,
        MonthOfYear = 2,
        HourOfDay = 3
    }
}
