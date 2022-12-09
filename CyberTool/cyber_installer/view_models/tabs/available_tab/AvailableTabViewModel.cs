﻿using cyber_base.implement.utils;
using cyber_base.view_model;
using cyber_installer.implement.modules.server_contact_manager;
using cyber_installer.model;
using cyber_installer.view.usercontrols.list_item.available_item.@base;
using cyber_installer.view.usercontrols.tabs;
using cyber_installer.view.usercontrols.tabs.@base;
using cyber_installer.view_models.tabs.installed_tab;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cyber_installer.view_models.tabs.available_tab
{
    internal class AvailableTabViewModel : BaseViewModel, IAvailableTabContext
    {
        private Task? _requestDataTask;
        private CancellationTokenSource? _requestDataTaskCancellationTokenSource;
        private bool _isLoading;

        [Bindable(true)]
        public RangeObservableCollection<AvailableItemViewModel> ItemsSource { get; set; } = new RangeObservableCollection<AvailableItemViewModel>();

        [Bindable(true)]
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                InvalidateOwn();
            }
        }
        public async void OnTabOpened(AvailableSoftwaresTab sender)
        {
            ItemsSource.Clear();
            IsLoading = true;
            _requestDataTaskCancellationTokenSource = new CancellationTokenSource();
            _requestDataTask = ServerContactManager.Current.RequestMultipleSoftwareInfoFromCyberServer(isForce: true
                , requestedCallback: (toolsSource) =>
                {
                    if (toolsSource != null)
                    {
                        GenerateAvailableTool(toolsSource);
                    }
                    IsLoading = false;
                }
                , cancellationToken: _requestDataTaskCancellationTokenSource.Token);
            await _requestDataTask;
        }


        public async void OnScrollDownToBottom(object sender)
        {
            if (_requestDataTask != null && _requestDataTask.IsCompleted
                || _requestDataTask == null)
            {
                IsLoading = true;
                _requestDataTaskCancellationTokenSource = new CancellationTokenSource();
                _requestDataTask = ServerContactManager.Current.RequestMultipleSoftwareInfoFromCyberServer(
                    requestedCallback: (toolsSource) =>
                    {
                        if (toolsSource != null)
                        {
                            GenerateAvailableTool(toolsSource);
                        }
                        IsLoading = false;
                    }
                    , cancellationToken: _requestDataTaskCancellationTokenSource.Token);
                await _requestDataTask;
            }
        }

        public void OnTabClosed(AvailableSoftwaresTab sender)
        {
            if (_requestDataTask != null
                && !_requestDataTask.IsCompleted
                && _requestDataTaskCancellationTokenSource != null)
            {
                _requestDataTaskCancellationTokenSource.Cancel();
            }
        }

        private void GenerateAvailableTool(ICollection<ToolVO> toolsSource)
        {
            foreach (var tool in toolsSource)
            {
                var availableItem = new AvailableItemViewModel(tool);
                ItemsSource.Add(availableItem);
            }
        }

        private ItemStatus GetItemStatus(ToolVO tool)
        {
            // TODO: Check item status here
            return ItemStatus.None;
        }


    }
}