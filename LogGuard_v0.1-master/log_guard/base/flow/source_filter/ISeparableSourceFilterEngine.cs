using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace log_guard.@base.flow.source_filter
{
    internal interface ISeparableSourceFilterEngine : IFilterEngine
    {
        ObservableCollection<string> SourceParts { get; }

        event SourcePartsCollectionChangedHandler PartsCollectionChanged;
    }

    public delegate void SourcePartsCollectionChangedHandler(object sender, NotifyCollectionChangedEventArgs args);
}
