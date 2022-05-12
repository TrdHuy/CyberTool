using log_guard.@base.watcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace log_guard.views.others.log_watcher._item
{
    [TemplatePart(Name = LogWatcherItem.RootBorderName, Type = typeof(Border))]
    [TemplatePart(Name = LogWatcherItem.IndicateSelectedPathName, Type = typeof(Path))]
    [TemplatePart(Name = LogWatcherItem.FeedbackBorderRecName, Type = typeof(Rectangle))]
    [TemplatePart(Name = LogWatcherItem.FeedbackRecName, Type = typeof(Rectangle))]
    [TemplatePart(Name = LogWatcherItem.GridRowPresenterName, Type = typeof(GridViewRowPresenter))]
    public class LogWatcherItem : ListViewItem
    {
        private const string RootBorderName = "PART_MainBorder";
        private const string IndicateSelectedPathName = "IndicateSelectedArrow";
        private const string FeedbackBorderRecName = "FeedbackBorder";
        private const string FeedbackRecName = "FeedbackRec";
        private const string GridRowPresenterName = "PART_RowPresenter";

        private ILogWatcherElements _context;

        private Border RootBorder { get; set; }
        private Path IndicatorPath { get; set; }
        private Rectangle FeedbackRec { get; set; }
        private Rectangle FeedbackBorderRec { get; set; }
        private GridViewRowPresenter RowPresenter { get; set; }
        public LogWatcherItem()
        {

        }

        internal void SetContext(ILogWatcherElements context)
        {
            _context = context;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            RootBorder = GetTemplateChild(RootBorderName) as Border;
            IndicatorPath = GetTemplateChild(IndicateSelectedPathName) as Path;
            FeedbackBorderRec = GetTemplateChild(FeedbackBorderRecName) as Rectangle;
            FeedbackRec = GetTemplateChild(FeedbackRecName) as Rectangle;
            RowPresenter = GetTemplateChild(GridRowPresenterName) as GridViewRowPresenter;
        }

     
    }
}
