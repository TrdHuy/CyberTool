using honeyboard_release_service.views.elements.calendar_notebook.@base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Data;

namespace honeyboard_release_service.views.elements.calendar_notebook.data_structure
{
    public class NotebookListObservableCollection<T> : ObservableCollection<T>
        where T : ObservableCollection<ICalendarNotebookCommitItemContext>
    {
        private object ThreadSafeLock = new object();
        public event SizeChangedHandler? SizeChanged;
        public int Size { get; private set; }


        public NotebookListObservableCollection(int size = 30)
        {
            BindingOperations.EnableCollectionSynchronization(this, ThreadSafeLock);

            Size = size;
            for (int i = 0; i < size; i++)
            {
                var item = (T)(new ObservableCollection<ICalendarNotebookCommitItemContext>());
                Items.Add(item);
            }
        }

        public void Replace(int index, T item)
        {
            if (index < Size && index >= 0)
            {
                SetItem(index, item);
            }
        }

        public void IncreaseSize(int amount, bool isNotify, bool isLast = true)
        {
            if (amount < 0)
            {
                throw new ArgumentException("amount must be greater than 0");
            }
            Size += amount;
            SizeChanged?.Invoke(this, Size - 1, Size);
            var listNewItem = new List<T>();

            if (isLast)
            {
                for (int i = 0; i < amount; i++)
                {
                    var item = (T)(new ObservableCollection<ICalendarNotebookCommitItemContext>());
                    Items.Add(item);
                    listNewItem.Add(item);
                }
            }
            else
            {
                for (int i = 0; i < amount; i++)
                {
                    var item = (T)(new ObservableCollection<ICalendarNotebookCommitItemContext>());
                    Items.Insert(0, item);
                    listNewItem.Insert(0, item);
                }
            }


            if (isNotify)
            {
                OnPropertyChanged(new PropertyChangedEventArgs("Count"));
                OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
                OnCollectionChanged(
                    new NotifyCollectionChangedEventArgs
                        (NotifyCollectionChangedAction.Add, listNewItem));
            }
        }

        public void DecreaseSize(int amount, bool isNotify, bool isLast = true)
        {
            if (amount < 0 || amount > Size)
            {
                throw new ArgumentException("amount must be greater than 0 and lower than collection size");
            }

            Size -= amount;
            SizeChanged?.Invoke(this, Size + 1, Size);
            var listOldItem = new List<T>();
            if (isLast)
            {
                for (int i = 0; i < amount; i++)
                {
                    var item = Items[Items.Count - 1];
                    Items.RemoveAt(Items.Count - 1);
                    listOldItem.Add(item);
                }
            }
            else
            {
                for (int i = 0; i < amount; i++)
                {
                    var item = Items[0];
                    Items.RemoveAt(0);
                    listOldItem.Add(item);
                }
            }


            if (isNotify)
            {
                OnPropertyChanged(new PropertyChangedEventArgs("Count"));
                OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
                OnCollectionChanged(
                    new NotifyCollectionChangedEventArgs
                        (NotifyCollectionChangedAction.Remove, listOldItem));
            }
        }

        public new void Add(T item)
        {
            Size += 1;
            SizeChanged?.Invoke(this, Size - 1, Size);
            base.Add(item);
        }

        public new void Insert(int index, T item)
        {
            Size += 1;
            SizeChanged?.Invoke(this, Size - 1, Size);
            base.Insert(index, item);
        }

        public new void Remove(T item)
        {
            if (Contains(item))
            {
                Size -= 1;
                SizeChanged?.Invoke(this, Size + 1, Size);
                base.Remove(item);
            }
        }
    }

    public delegate void SizeChangedHandler(object sender, int oldSize, int newSize);
}
