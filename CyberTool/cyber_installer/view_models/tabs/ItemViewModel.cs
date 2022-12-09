﻿using cyber_base.view_model;
using cyber_installer.model;
using cyber_installer.view.usercontrols.list_item.available_item.@base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_installer.view_models.tabs
{
    internal abstract class ItemViewModel : BaseViewModel, IItemContext
    {
        private Uri? _iconSource;
        private ItemStatus _itemStatus;
        private string _version = "";
        private string _softwareName = "";
        private double _swHandlingProgress = 0d;
        private bool _isLoadingItemStatus = false;
        protected ToolVO _toolVO;

        public ToolVO ToolInfo
        {
            get => _toolVO;
        }

        public ItemStatus ItemStatus
        {
            get => _itemStatus;
            set
            {
                _itemStatus = value;
                InvalidateOwn();
            }
        }

        public Uri IconSource
        {
            get => _iconSource ?? new Uri("");
            set
            {
                _iconSource = value;
                InvalidateOwn();
            }
        }

        public string Version
        {
            get => _version;
            set
            {
                _version = value;
                InvalidateOwn();
            }
        }

        public string SoftwareName
        {
            get => _softwareName;
            set
            {
                _softwareName = value;
                InvalidateOwn();
            }
        }

        public double SwHandlingProgress
        {
            get => _swHandlingProgress;
            set
            {
                _swHandlingProgress = value;
                InvalidateOwn();
            }
        }

        public bool IsLoadingItemStatus
        {
            get => _isLoadingItemStatus;
            set
            {
                _isLoadingItemStatus = value;
                InvalidateOwn();
            }
        }

        public ItemViewModel(ToolVO toolVO)
        {
            _toolVO = toolVO;
            _softwareName = toolVO.Name;
            _version = toolVO.ToolVersions.Last().Version;
            _iconSource = new Uri(toolVO.IconSource);
            InstantiateItemStatus();
        }

        protected abstract void InstantiateItemStatus();
    }
}

