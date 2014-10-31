using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GdiPresentation
{
    internal abstract class LayoutQueue<T>
    {
        private const int QueueSize = 50;

        private int _head;
        private readonly T[] _queue = new T[QueueSize];

        public int Count
        {
            get { return _head; }
        }

        public bool IsFull
        {
            get { return _head == QueueSize; }
        }

        public bool IsEmpty
        {
            get { return _head == 0; }
        }

        protected abstract void ItemAdded();

        public void Reset()
        {
            _head = 0;
        }

        public void Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (_head == QueueSize)
                return;

            _queue[_head++] = item;

            ItemAdded();
        }

        public T GetItem(int index)
        {
            Debug.Assert(index < _head);

            return _queue[index];
        }
    }
}
