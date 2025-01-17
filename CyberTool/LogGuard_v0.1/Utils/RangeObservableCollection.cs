﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LogGuard_v0._1.Utils
{
    public class RangeObservableCollection<T> : ObservableCollection<T>
    {
        public object ThreadSafeLock = new object();

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