using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;

namespace cyber_base.implement.utils
{
    public class RangeObservableCollection<T> : ObservableCollection<T>
    {
        private static Logger ROCLogger = new Logger("RangeObservableCollection");

        public object ThreadSafeLock = new object();
        private SemaphoreSlim _addAsyncNewRangeSemaphore = new SemaphoreSlim(1, 1);

        public RangeObservableCollection()
        {
            BindingOperations.EnableCollectionSynchronization(this, ThreadSafeLock);
            //BindingOperations.AccessCollection(this, new System.Action(() => { DataGridBehavior.DataGridBehavior.ScrollToEnd(); }), true);
        }

        public RangeObservableCollection(IEnumerable<T> e) : base(e)
        {
            BindingOperations.EnableCollectionSynchronization(this, ThreadSafeLock);
        }

        public RangeObservableCollection(List<T> e) : base(e)
        {
            BindingOperations.EnableCollectionSynchronization(this, ThreadSafeLock);
        }

        public void AddRange(IEnumerable<T> list)
        {
            if (list == null)
                return;

            foreach (T item in list)
                Items.Add(item);
            SendNotifications();
        }

        public void AddWithoutNotify(T item)
        {
            Items.Add(item);
        }

        public void InsertRange(IEnumerable<T> list, int index)
        {
            if (list == null)
                return;

            foreach (T item in list)
                Items.Insert(++index, item);
            SendNotifications();
        }

        public void AddNewRange(IEnumerable<T> list)
        {
            if (list == null)
                return;
            Items.Clear();
            foreach (T item in list)
                Items.Add(item);
            SendNotifications();
        }

        public async Task AsyncAddNewRange(IAsyncEnumerable<T> list
            , Action? asyncCollectionChangedCallback = null)
        {
            if (list == null)
                return;

            await _addAsyncNewRangeSemaphore.WaitAsync();
            
            Items.Clear();
            try
            {
                await foreach (T item in list)
                {
                    Items.Add(item);
                    asyncCollectionChangedCallback?.Invoke();
                    SendNotifications();
                }
            }
            catch (OperationCanceledException e)
            {
                ROCLogger.E(e.ToString());
            }
            finally
            {
                _addAsyncNewRangeSemaphore.Release();
            }

        }

        public void RemoveRange(IEnumerable<T> list)
        {
            if (list == null)
                return;

            foreach (T item in list)
                Items.Remove(item);
            SendNotifications();
        }

        public void SendNotifications()
        {
            OnCollectionChanged(new RangeObservableCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, Items.Count));
            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new PropertyChangedEventArgs("Items"));
        }

        public void SendItemsChangedNotifications()
        {
            OnPropertyChanged(new PropertyChangedEventArgs("Items"));
        }
    }

    public class RangeObservableCollectionChangedEventArgs : NotifyCollectionChangedEventArgs
    {
        public int NewCount { get; }
        public RangeObservableCollectionChangedEventArgs(NotifyCollectionChangedAction action, int count) : base(action)
        {
            NewCount = count;
        }
    }
}