using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace honeyboard_release_service.utils
{
    internal class FirstLastObservableCollection<T> : ObservableCollection<T>
    {
        private T? _first;
        private T? _last;

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
                _first = value;
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
                _last = value;
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
        }

        private void HandleFirstLast(object? sender, NotifyCollectionChangedEventArgs e)
        {
            First = this[0];
            Last = this[Count - 1];
        }
    }

    internal delegate void FirstChangedHandler<T>(object sender, T? oldFirst, T? newFirst);
    internal delegate void LastChangedHandler<T>(object sender, T? oldLast, T? newLast);
}
