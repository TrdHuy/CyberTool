using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace LogGuard_v0._1.Base.LogGuardFlow.SourceFilter
{
    public interface ISeparableSourceFilterEngine : IFilterEngine
    {
        ObservableCollection<string> SourceParts { get; }

        event SourcePartsCollectionChangedHandler PartsCollectionChanged;
    }

    public delegate void SourcePartsCollectionChangedHandler(object sender, NotifyCollectionChangedEventArgs args);
}
