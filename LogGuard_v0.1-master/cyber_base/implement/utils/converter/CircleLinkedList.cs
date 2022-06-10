using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_base.implement.utils.converter
{
    public class CircleLinkedList<T>
    {
        class CircleNode
        {
            public T Value;
            public CircleNode Next;
            public CircleNode Previous;

            public CircleNode(T val)
            {
                Value = val;
                Next = this;
                Previous = this;
            }
        }

        public T Head { get => _head.Value; }
        public T Last { get => _last.Value; }
        public int Count { get; private set; }

        private CircleNode _current;
        private CircleNode _head;
        private CircleNode _last;

        private Dictionary<int, CircleNode> _nodeMap;

        private int GetHashId(T item)
        {
            return item?.GetHashCode() ?? 0;
        }

        public CircleLinkedList(T head)
        {
            _nodeMap = new Dictionary<int, CircleNode>();
            _current = new CircleNode(head);
            _current.Next = _current;
            _current.Previous = _current;
            _head = _current;
            _last = _current;
            Count++;
            _nodeMap.Add(GetHashId(head), _current);
        }

        public bool IsEmpty()
        {
            return Count == 0;
        }

        public void Add(T item)
        {
            var newNode = new CircleNode(item);
            newNode.Next = _head;
            newNode.Previous = _current;
            _current.Next = newNode;

            if (_current == _head)
            {
                _current.Previous = newNode;
            }
            _last = newNode;
            _current = _last;

            Count++;
            _nodeMap.Add(GetHashId(item), _current);
        }

        public void Remove(T item)
        {
            if (Count == 1) return;
            var id = GetHashId(item);
            if (_nodeMap.ContainsKey(id))
            {
                var rmNode = _nodeMap[id];
                var preNode = rmNode.Previous;
                var nextNode = rmNode.Next;

                preNode.Next = nextNode;
                nextNode.Previous = preNode;

                if (rmNode == _last)
                {
                    _last = rmNode.Previous;
                }
                else if (rmNode == _head)
                {
                    _head = rmNode.Next;
                }
                _current = _last;

                _nodeMap.Remove(id);
                Count--;
            }
        }

        public T? Next(T item)
        {
            if (_nodeMap.ContainsKey(GetHashId(item)))
            {
                return _nodeMap[GetHashId(item)].Next.Value;
            }
            return default(T);
        }
    }
}
