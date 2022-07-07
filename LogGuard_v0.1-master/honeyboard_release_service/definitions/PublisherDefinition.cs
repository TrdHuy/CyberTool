using System;
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
        public const string PUBLISHER_PLUGIN_TAG = "SWPublisherPlugin";

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
