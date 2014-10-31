using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GdiPresentation
{
    internal class ElementEnumerable : IEnumerable<Element>
    {
        private readonly Element _start;
        private readonly bool _skipSelf;
        private readonly bool _forward;
        private readonly bool _wrap;

        public ElementEnumerable(Element start, bool skipSelf, bool forward, bool wrap)
        {
            if (start == null)
                throw new ArgumentNullException("start");

            _start = start;
            _skipSelf = skipSelf;
            _forward = forward;
            _wrap = wrap;
        }

        public IEnumerator<Element> GetEnumerator()
        {
            return new ElementEnumerator(_start, _skipSelf, _forward, _wrap);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class ElementEnumerator : IEnumerator<Element>
        {
            private readonly Element _start;
            private readonly bool _skipSelf;
            private readonly bool _forward;
            private readonly bool _wrap;
            private readonly Element _root;
            private bool _eof;

            public ElementEnumerator(Element start, bool skipSelf, bool forward, bool wrap)
            {
                _start = start;
                _skipSelf = skipSelf;
                _forward = forward;
                _wrap = wrap;
                _root = start.LayoutManager.Host.Content;

                Reset();
            }

            public Element Current { get; private set; }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                if (_eof)
                    return false;

                // If we're starting fresh (and we're allowed to return the
                // start), return the start.

                if (Current == null)
                {
                    Current = _start;
                    return true;
                }

                // Execute the algorithm.

                bool result;

                if (_forward)
                    result = MoveNextForward();
                else
                    result = MoveNextBackward();

                // If we couldn't find the next element, or we're back
                // at the beginning, we're done.

                if (!result || Current == _start)
                {
                    Current = null;
                    _eof = true;
                }

                return !_eof;
            }

            private bool MoveNextForward()
            {
                // If we have children, recurs into the children.

                if (Current.GetChildrenCount() > 0)
                {
                    Current = Current.GetChild(0);
                    return true;
                }

                // Find a parent of which we aren't the last child.

                var parent = Current.Parent;

                while (Current != _root)
                {
                    int parentCount = parent.GetChildrenCount();
                    int index = parent.IndexOf(Current);

                    Debug.Assert(index != -1);

                    if (index < parentCount - 1)
                    {
                        Current = parent.GetChild(index + 1);
                        return true;
                    }

                    Current = parent;
                    parent = parent.Parent;
                }

                // If we're at the end, and we're allowed to wrap, start
                // at the root element.

                return _wrap;
            }

            private bool MoveNextBackward()
            {
                // Are we at the root element?

                if (Current == _root)
                {
                    // If we're allowed to wrap, we move to the last child in
                    // the tree.

                    if (_wrap || Current == _start)
                    {
                        MoveToLastChildRecursive();
                        return true;
                    }

                    return false;
                }

                // If we're the first child of our parent, move to our parent.

                int index = Current.Parent.IndexOf(Current);

                if (index == 0)
                {
                    Current = Current.Parent;
                    return true;
                }

                // Otherwise, move to our previous sibling and select the
                // deepest child.

                Current = Current.Parent.GetChild(index - 1);
                MoveToLastChildRecursive();

                return true;
            }

            private void MoveToLastChildRecursive()
            {
                // This finds the deepest nested last child of the current
                // element.

                int count = Current.GetChildrenCount();

                while (count > 0)
                {
                    Current = Current.GetChild(count - 1);

                    count = Current.GetChildrenCount();
                }
            }

            public void Reset()
            {
                _eof = false;

                Current = _skipSelf ? _start : null;
            }

            public void Dispose()
            {
                // Nothing to do.
            }
        }
    }
}
