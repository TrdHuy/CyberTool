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
        public const string PUBLISHER_PLUGIN_TAG = "SWPublisherPlugin";
        public const string PUBLISHER_PLUGIN_GEOMETRY_DATA = "m 13.50761,49.301343 c 0.1102,-0.38426 0.419899,-2.079129 0.688212,-3.766375 C 14.854211,41.394837 14.912035,33.048603 14.303069,30.05637 13.363278,25.438557 11.188318,19.215891 8.8875176,14.562178 L 7.8182336,12.399401 V 8.1789796 c 0,-2.3212306 0.09387,-4.4673116 0.2086,-4.7690666 C 8.1415556,3.108157 8.4608846,2.713738 8.7364356,2.533426 9.2254386,2.213444 25.603683,0 27.482372,0 c 0.634647,0 0.93394,0.134443 1.280715,0.575297 0.420725,0.534862 0.452529,0.858818 0.452529,4.609264 v 4.0339676 l 1.595439,1.6959604 c 3.990562,4.241998 7.517555,11.136123 8.575399,16.762145 0.481519,2.560899 0.330783,7.310421 -0.307773,9.697915 -0.520873,1.947473 -1.666813,4.791467 -2.187769,5.429594 -0.267571,0.327759 -3.322543,1.31519 -11.815307,3.818958 C 18.775669,48.480396 13.550538,50 13.464203,50 c -0.08634,0 -0.06684,-0.314394 0.04341,-0.698657 z M 19.659526,8.3114276 c 2.009438,-0.357607 3.859681,-0.760529 4.111655,-0.895382 C 24.336955,7.1132526 24.814323,6.031581 24.683706,5.348332 24.556638,4.683598 23.880515,3.969426 23.22173,3.804081 22.933925,3.731851 20.810037,4.017748 18.501975,4.439419 13.926799,5.275278 13.577746,5.435006 13.357262,6.79369 c -0.20213,1.2455766 1.049158,2.4447826 2.321233,2.2246166 0.180138,-0.03118 1.971593,-0.349272 3.981031,-0.706879 z M 7.7090646,44.961071 C 4.9470546,42.323564 2.2553876,39.734507 1.7275736,39.20761 L 0.7679206,38.249614 0.4672826,35.55494 c -0.36512,-3.27261 -0.62075,-13.939796 -0.362042,-15.107567 l 0.186278,-0.840838 6.262661,4.770969 6.2626594,4.770968 0.01164,10.316594 c 0.0066,5.674127 -0.01296,10.310941 -0.043,10.304033 -0.03004,-0.0069 -2.314415,-2.170522 -5.0764254,-4.808028 z M 6.5081896,23.345605 c -3.3024,-2.397978 -6.038745,-4.460858 -6.080765,-4.584179 -0.154862,-0.454455 1.076844,-1.935902 2.464945,-2.964751 1.381781,-1.024159 4.27576,-2.772072 4.384203,-2.647978 0.23027,0.263505 1.605787,3.239664 2.37965,5.148755 1.4301884,3.528229 3.3096934,9.448063 2.9907694,9.419959 -0.07394,-0.0065 -2.8363944,-1.973827 -6.1388024,-4.371806 z";
    }

    internal class PublisherKeyFeatureTag
    {
        public const string KEY_TAG_PRT_QUICK_RELEASE_FEATURE = "KEY_TAG_PRT_QUICK_RELEASE_FEATURE";
        public const string KEY_TAG_PRT_CREATE_CL_AND_COMMIT_FEATURE = "KEY_TAG_PRT_CREATE_CL_AND_COMMIT_FEATURE";
        public const string KEY_TAG_PRT_CLEAN_RELEASE_INFORMATION_FEATURE = "KEY_TAG_PRT_CLEAN_RELEASE_INFORMATION_FEATURE";
        public const string KEY_TAG_PRT_SAVE_RELEASE_TEMPLATE_FEATURE = "KEY_TAG_PRT_SAVE_RELEASE_TEMPLATE_FEATURE";
        public const string KEY_TAG_PRT_RESTORE_LATEST_RELEASE_FEATURE = "KEY_TAG_PRT_RESTORE_LATEST_RELEASE_FEATURE";

        public const string KEY_TAG_PRT_PM_FETCH_PROJECT_FEATURE = "KEY_TAG_PRT_PM_FETCH_PROJECT_FEATURE";
        public const string KEY_TAG_PRT_PM_PROJECT_PATH_SELECTED_FEATURE = "KEY_TAG_PRT_PM_PROJECT_PATH_SELECTED_FEATURE";
        public const string KEY_TAG_PRT_PM_SELECTED_BRANCH_CHANGED_FEATURE = "KEY_TAG_PRT_PM_SELECTED_BRANCH_CHANGED_FEATURE";

        public const string KEY_TAG_PRT_LM_CLEAR_LOG_CONTENT_FEATURE = "KEY_TAG_PRT_LM_CLEAR_LOG_CONTENT_FEATURE";
        public const string KEY_TAG_PRT_LM_CLIPBOARD_LOG_CONTENT_FEATURE = "KEY_TAG_PRT_LM_CLIPBOARD_LOG_CONTENT_FEATURE";

        public const string KEY_TAG_PRT_SWITCH_CALENDAR_FEATURE = "KEY_TAG_PRT_SWITCH_CALENDAR_FEATURE";
        public const string KEY_TAG_PRT_SWITCH_LOG_MONITOR_FEATURE = "KEY_TAG_PRT_SWITCH_LOG_MONITOR_FEATURE";

    }

    internal enum PublisherViewKeyDefinition
    {
        CalendarNoteBookBorder = 1,
        CalendarNoteBookGridContainer = 2,
        CalendarNoteBookListView = 3,
    }
}
