using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_base.implement.utils
{
    public class AutoResizeStack<T>
    {
        private LinkedList<T> _storage;
        private int _capacity;
        public int Count { get => _storage.Count; }

        public AutoResizeStack(int size = 5)
        {
            _storage = new LinkedList<T>();
            _capacity = size;
        }

        public void Push(T data)
        {
            if (_storage.Count < _capacity)
            {
                _storage.AddLast(data);
            }
            else
            {
                _storage.RemoveFirst();
                _storage.AddLast(data);
            }
        }

        public T? Pop()
        {
            if (_storage.Count == 0) return default(T);
            if (_storage.Last == null) return default(T);

            var last = _storage.Last.Value;
            _storage.RemoveLast();
            return last;
        }

        public T? Peek()
        {
            if (_storage.Count == 0) return default(T);
            if (_storage.Last == null) return default(T);
            return _storage.Last.Value;
        }
    }
}
