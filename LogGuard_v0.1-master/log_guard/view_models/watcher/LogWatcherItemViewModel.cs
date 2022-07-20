using cyber_base.view_model;
using log_guard.@base.watcher;
using log_guard.definitions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.view_models.watcher
{
    public class LogWatcherItemViewModel : BaseViewModel, ILogWatcherElements
    {
        private Color _error = Color.Red;
        private Color _track = Color.Gray;

        private ElementViewType _viewType;

        private int _lineNumber = -1;

        public ElementViewType ViewType { get => _viewType; set => _viewType = value; }


        /// <summary>
        /// Thuộc tính quan trong nhất trong tính năng delete log
        /// Thuộc tính này khác với thuộc tính Line
        /// Line là vị trí của dòng log trong file raw text hoặc từ process capture log
        /// Thuộc tính này chỉ ra vị trí hiển thị của dòng log hiện tại 
        /// đang ở vị trí nào trong LogWatcher
        /// 
        /// Chỉ cập nhật lại thuộc tính này khi dòng log đươc đưa vào lại display source
        /// (source này dưới sự quản lý của SourceManager)
        /// </summary>
        public int LineNumber { get => _lineNumber; set => _lineNumber = value; }


        public virtual Color? TrackColor { get => _track; set => _track = value ?? Color.Gray; }

        public virtual Color? ErrorColor { get => _error; set => _error = value ?? Color.Red; }
    }
}