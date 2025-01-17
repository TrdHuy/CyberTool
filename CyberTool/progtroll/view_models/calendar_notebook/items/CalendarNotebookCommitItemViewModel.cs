﻿using cyber_base.view_model;
using progtroll.models.VOs;
using progtroll.view_models.project_manager.items;
using progtroll.views.elements.calendar_notebook.@base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progtroll.view_models.calendar_notebook.items
{
    internal class CalendarNotebookCommitItemViewModel : BaseViewModel, ICalendarNotebookCommitItemContext
    {
        private ICalendarNotebookProjectItemContext _project;
        private VersionUpCommitVO _commitVO;
        private string _mainContent = "";

        [Bindable(true)]
        public DateTime TimeId
        {
            get
            {
                return _commitVO.ReleaseDateTime;
            }
            set
            {
                _commitVO.ReleaseDateTime = value;
            }
        }

        [Bindable(true)]
        public string MainContent
        {
            get
            {
                return _mainContent;
            }
            set
            {
                _mainContent = value;
            }
        }

        public ICalendarNotebookProjectItemContext Project => _project;

        public CalendarNotebookCommitItemViewModel(VersionUpCommitVO vo
            , ICalendarNotebookProjectItemContext project)
        {
            _commitVO = vo;
            _mainContent = vo.CommitTitle;
            _project = project;
        }
    }
}
