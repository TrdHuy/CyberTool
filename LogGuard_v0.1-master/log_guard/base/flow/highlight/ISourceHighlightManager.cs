using log_guard.@base.flow.source_filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.@base.flow.highlight
{
    internal interface ISourceHighlightManager
    {
        event SourceHighlightConditionChangedHandler HighlightConditionChanged;

        /// <summary>
        /// Highlight các chuỗi cần thiết trong đối tượng đã được filter
        /// </summary>
        /// <param name="obj">đối tượng cần được highlight</param>
        /// <returns></returns>
        void FilterHighlight(object obj);

        /// <summary>
        /// Highlight các chuỗi cần thiết trong đối tượng đã được tìm kiếm bởi Finder
        /// </summary>
        /// <param name="obj">đối tượng cần được highlight</param>
        /// <returns></returns>
        void FinderHighlight(object obj);

        /// <summary>
        /// Làm mới đối tượng trước khi highlight 
        /// </summary>
        /// <param name="obj">đối tượng cần được làm mới</param>
        /// <returns></returns>
        void Clean(object obj);

        ISourceHighlightor MessageFilterHighlightor { get; set; }
        ISourceHighlightor FinderHighlightor { get; set; }
        ISourceHighlightor TagFilterHighlightor { get; set; }

        void NotifyHighlightPropertyChanged(ISourceHighlightor sender, object e);

    }

    internal delegate void SourceHighlightConditionChangedHandler(object sender, ConditionChangedEventArgs e);

}
