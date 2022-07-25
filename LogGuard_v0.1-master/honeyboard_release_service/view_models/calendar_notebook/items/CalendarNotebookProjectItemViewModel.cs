using cyber_base.implement.command;
using cyber_base.view_model;
using honeyboard_release_service.implement.view_model;
using honeyboard_release_service.models.VOs;
using honeyboard_release_service.views.elements.calendar_notebook.@base;
using honeyboard_release_service.views.elements.calendar_notebook.data_structure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace honeyboard_release_service.view_models.calendar_notebook.items
{
    internal class CalendarNotebookProjectItemViewModel : BaseViewModel, ICalendarNotebookProjectItemContext
    {
        private ProjectVO _projectVO;
        private object? _itemView;
        private bool _isLoadingData;
        private CalendarNotebookItemCollection<ICalendarNotebookCommitItemContext> _commitSource;
        private ICommand _renameProjectCommand;
        private ICommand _deleteProjectCommand;

        public CalendarNotebookItemCollection<ICalendarNotebookCommitItemContext> CommitSource => _commitSource;

        [Bindable(true)]
        public string ProjectName
        {
            get
            {
                return _projectVO.Name;
            }
            set
            {
                _projectVO.Name = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public object? ItemView
        {
            get
            {
                return _itemView;
            }
            set
            {
                _itemView = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public bool IsLoadingData
        {
            get
            {
                return _isLoadingData;
            }
            set
            {
                _isLoadingData = value;
                InvalidateOwn();
            }
        }

        public ICommand RenameProjectCommand { get => _renameProjectCommand; }
        public ICommand DeleteProjectCommand { get => _deleteProjectCommand; }

        public CalendarNotebookProjectItemViewModel(ProjectVO vo)
        {
            _commitSource = new CalendarNotebookItemCollection<ICalendarNotebookCommitItemContext>();
            _projectVO = vo;
            _renameProjectCommand = new CommandExecuterModel((paramaters) =>
            {
                return null;
            });
            _deleteProjectCommand = new CommandExecuterModel((paramaters) =>
            {
                var cnViewmodel = ViewModelManager.Current.CNViewModel;
                cnViewmodel.DeleteProjectItem(_projectVO);
                return null;
            });
        }

        public async override void OnBegin()
        {
            if (_projectVO.OnBranch != null
                && _projectVO.OnBranch.CommitMap != null)
            {
                IsLoadingData = true;
                await Task.Delay(2000);
                foreach (var commitVO in _projectVO.OnBranch.CommitMap.Values)
                {
                    CommitSource.Add(new CalendarNotebookCommitItemViewModel(commitVO
                       , this));
                }
                IsLoadingData = false;
            }
            else
            {
                IsLoadingData = true;
            }
        }
    }
}
