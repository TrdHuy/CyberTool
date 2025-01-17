﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_installer.view.usercontrols.list_item.available_item.@base
{
    internal interface IItemContext
    {
        public ItemStatus ItemStatus { get; set; }
        public Uri IconSource { get; set; }
        public string Version { get; set; }
        public string SoftwareName { get; set; }
        public double SwHandlingProgress { get; set; }
        public bool IsLoadingItemStatus { get; set; }
    }
}
