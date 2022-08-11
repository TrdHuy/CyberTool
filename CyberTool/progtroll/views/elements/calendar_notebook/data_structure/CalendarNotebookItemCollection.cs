using progtroll.views.elements.calendar_notebook.@base;
using progtroll.views.elements.calendar_notebook.definitions;
using progtroll.views.elements.calendar_notebook.extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace progtroll.views.elements.calendar_notebook.data_structure
{
    public class CalendarNotebookItemCollection<T> : ObservableCollection<T>
        where T : ICalendarNotebookCommitItemContext
    {
        private object ThreadSafeLock = new object();
        private Dictionary<DateTime, ObservableCollection<T>> _dayTimeMap = new Dictionary<DateTime, ObservableCollection<T>>();
        private Dictionary<DateTime, ObservableCollection<T>> _monthTimeMap = new Dictionary<DateTime, ObservableCollection<T>>();
        public event KeyCollectionChangedHandler<T>? KeyCollectionChanged;

        public CalendarNotebookItemCollection()
        {
            BindingOperations.EnableCollectionSynchronization(this, ThreadSafeLock);
        }

        public ObservableCollection<T>? this[DateTime key, CalendarNotebookDateMode dateMode]
        {
            get
            {
                try
                {
                    if (dateMode == CalendarNotebookDateMode.Day)
                    {
                        return _dayTimeMap[key];
                    }
                    else
                    {
                        var newKey = key.StartOfMonth();
                        return _monthTimeMap[newKey];
                    }
                }
                catch
                {
                    return null;
                }
            }
        }

        // Thêm item vào map với key là date
        // từ TimeId của Item T 
        public new void Add(T item)
        {
            var dayKey = item.TimeId.Date;
            var monthKey = item.TimeId.StartOfMonth();
            if (_dayTimeMap.ContainsKey(dayKey))
            {
                _dayTimeMap[dayKey].Add(item);
            }
            else
            {
                _dayTimeMap[dayKey] = new ObservableCollection<T>();
                _dayTimeMap[dayKey].Add(item);
                KeyCollectionChanged?.Invoke(this, new KeyCollectionChangedEventArgs<T>(dayKey
                    , null
                    , KeyCollectionChangedEventArgs<T>.Action.Add
                    , item
                    , KeyCollectionChangedEventArgs<T>.Key.Day));
            }

            if (_monthTimeMap.ContainsKey(monthKey))
            {
                _monthTimeMap[monthKey].Add(item);
            }
            else
            {
                _monthTimeMap[monthKey] = new ObservableCollection<T>();
                _monthTimeMap[monthKey].Add(item);
                KeyCollectionChanged?.Invoke(this, new KeyCollectionChangedEventArgs<T>(monthKey
                    , null
                    , KeyCollectionChangedEventArgs<T>.Action.Add
                    , item
                    , KeyCollectionChangedEventArgs<T>.Key.Month));
            }
            base.Add(item);
        }

        // Xóa item từ map với key là date
        // từ TimeId của Item T 
        public new void Remove(T item)
        {
            var dayKey = item.TimeId.Date;
            var monthKey = item.TimeId.StartOfMonth();

            if (_dayTimeMap.ContainsKey(dayKey))
            {
                _dayTimeMap[dayKey].Remove(item);
                if (_dayTimeMap[dayKey].Count == 0)
                {
                    _dayTimeMap.Remove(dayKey);
                    KeyCollectionChanged?.Invoke(this, new KeyCollectionChangedEventArgs<T>(null
                        , dayKey
                        , KeyCollectionChangedEventArgs<T>.Action.Remove
                        , item
                        , KeyCollectionChangedEventArgs<T>.Key.Day));
                }
            }

            if (_monthTimeMap.ContainsKey(monthKey))
            {
                _monthTimeMap[monthKey].Remove(item);
                if (_monthTimeMap[monthKey].Count == 0)
                {
                    _monthTimeMap.Remove(monthKey);
                    KeyCollectionChanged?.Invoke(this, new KeyCollectionChangedEventArgs<T>(null
                        , monthKey
                        , KeyCollectionChangedEventArgs<T>.Action.Remove
                        , item
                        , KeyCollectionChangedEventArgs<T>.Key.Month));
                }
            }
            base.Remove(item);
        }

        public new void Clear()
        {
            _dayTimeMap.Clear();
            _monthTimeMap.Clear();
            KeyCollectionChanged?.Invoke(this, new KeyCollectionChangedEventArgs<T>(null
                , null
                , KeyCollectionChangedEventArgs<T>.Action.Reset
                , default(T)
                , KeyCollectionChangedEventArgs<T>.Key.All));
            base.Clear();
        }
    }

    public delegate void KeyCollectionChangedHandler<T>(object sender, KeyCollectionChangedEventArgs<T> e)
        where T : ICalendarNotebookCommitItemContext;

    public class KeyCollectionChangedEventArgs<T>
        where T : ICalendarNotebookCommitItemContext
    {
        public enum Key
        {
            All = 0,
            Day = 1,
            Month = 2,
        }
        public enum Action
        {
            Add = 1,
            Remove = 2,
            Reset = 3
        }

        public DateTime? NewKey { get; private set; }
        public DateTime? OldKey { get; private set; }
        public Action ChangedAction { get; private set; }
        public T? ChangedItem { get; private set; }
        public Key KeyType { get; private set; }

        public KeyCollectionChangedEventArgs(DateTime? newKey
            , DateTime? oldKey
            , Action changedAction
            , T? changedItem
            , Key keyType)
        {
            NewKey = newKey;
            OldKey = oldKey;
            ChangedAction = changedAction;
            ChangedItem = changedItem;
            KeyType = keyType;
        }
    }

}
