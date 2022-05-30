using cyber_base.view_model;
using honeyboard_release_service.implement.view_manager.notebook_item;
using honeyboard_release_service.models.VOs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace honeyboard_release_service.view_models.calendar_notebook.items
{
    internal class NotebookItemViewModel : BaseViewModel
    {
        private object? _noteBookContent;
        private ProjectVO _projectVO;

        [Bindable(true)]
        public object? NoteBookContent
        {
            get
            {
                return _noteBookContent;
            }
            set
            {
                _noteBookContent = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string DisplayName
        {
            get
            {
                return _projectVO.Name;
            }
        }

        public NotebookItemViewModel(BaseViewModel parents, ProjectVO vo) : base(parents)
        {
            _projectVO = vo;
        }

        public override void OnViewInstantiated()
        {
            base.OnViewInstantiated();

            CalendarNotebookItemViewManager.Current.UpdateColumnNumber(
                ((CalendarNoteBookViewModel)ParentsModel).StartDateTime,
                ((CalendarNoteBookViewModel)ParentsModel).EndDateTime);
            NoteBookContent = CalendarNotebookItemViewManager.Current.GetItemContent();
            CalendarNotebookItemViewManager.Current.UpdateNotebookItemContent(_projectVO);
        }
    }
}
