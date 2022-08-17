using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace cyber_base.implement.utils
{
    public class FirstLastObservableCollection<T> : ObservableCollection<T>
        where T : IFirstLastElement
    {
        private T? _first;
        private T? _last;
        private object ThreadSafeLock = new object();

        public event FirstChangedHandler<T>? FirstChanged;
        public event LastChangedHandler<T>? LastChanged;

        public T? First
        {
            get
            {
                return _first;
            }
            set
            {
                var oldVal = _first;
                if (oldVal != null)
                {
                    oldVal.IsFirst = false;
                }
                _first = value;
                if (_first != null)
                {
                    _first.IsFirst = true;
                }

                if (oldVal == null && _first != null)
                {
                    FirstChanged?.Invoke(this, oldVal, value);
                }
                else if (oldVal != null && !oldVal.Equals(_first))
                {
                    FirstChanged?.Invoke(this, oldVal, value);
                }
            }
        }

        public T? Last
        {
            get
            {
                return _last;
            }
            set
            {
                var oldVal = _last;
                if (oldVal != null)
                {
                    oldVal.IsLast = false;
                }
                _last = value;
                if (_last != null)
                {
                    _last.IsLast = true;
                }

                if (oldVal == null && _last != null)
                {
                    LastChanged?.Invoke(this, oldVal, value);
                }
                else if (oldVal != null && !oldVal.Equals(_last))
                {
                    LastChanged?.Invoke(this, oldVal, value);
                }
            }
        }

        public FirstLastObservableCollection()
        {
            CollectionChanged -= HandleFirstLast;
            CollectionChanged += HandleFirstLast;
            BindingOperations.EnableCollectionSynchronization(this, ThreadSafeLock);

        }

        private void HandleFirstLast(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (Count > 0)
            {
                First = this[0];
                Last = this[Count - 1];
            }
        }

        public new void Clear()
        {
            _first = default(T);
            _last = default(T);
            base.Clear();
        }
    }

    public delegate void FirstChangedHandler<T>(object sender, T? oldFirst, T? newFirst);
    public delegate void LastChangedHandler<T>(object sender, T? oldLast, T? newLast);

    public interface IFirstLastElement
    {
        bool IsFirst { get; set; }
        bool IsLast { get; set; }
    }
}
