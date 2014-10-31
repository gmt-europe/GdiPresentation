using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GdiPresentation
{
    partial class Element
    {
        /// <remarks>
        /// This is a key/value store implementation using an array as storage
        /// with fallback to a hash table. Since we currently only have four
        /// attached properties (the ones for Grid), the provided array length
        /// should suffice in all circumstances.
        /// 
        /// The reason behind this implementation is that a sequential array
        /// search is about 8 times as fast (worst case; not in the list) as
        /// the hash table implementation. Since attached properties are
        /// looked up quite often, the increase of this pattern is significant.
        /// </remarks>
        private class AttachedProperties
        {
            private const int Storage = 4;

            private object[] _array;
            private Hashtable _table;

            public object Find(object key)
            {
                if (key == null)
                    throw new ArgumentNullException("key");

                if (_array != null)
                {
                    int index = FindIndex(key);

                    if (index != -1)
                        return _array[index + 1];
                }
                else if (_table != null)
                {
                    return _table[key];
                }

                return null;
            }

            private int FindIndex(object key)
            {
                for (int i = 0; i < _array.Length; i += 2)
                {
                    if (_array[i] == key)
                        return i;
                }

                return -1;
            }

            public void Set(object key, object value)
            {
                if (_array == null && _table == null)
                    _array = new object[Storage * 2];

                if (_array != null)
                {
                    int index = FindIndex(key);

                    // Overwrite an existing entry.

                    if (index != -1)
                    {
                        _array[index + 1] = value;
                        return;
                    }

                    // Find an empty slot.

                    for (int i = 0; i < _array.Length; i += 2)
                    {
                        if (_array[i] == null)
                        {
                            _array[i] = key;
                            _array[i + 1] = value;
                            return;
                        }
                    }

                    // The array is full; we're moving to a hash table.

                    Debug.Fail("Increase array length if possible");

                    _table = new Hashtable();

                    for (int i = 0; i < _array.Length; i += 2)
                    {
                        _table[_array[i]] = _array[i + 1];
                    }
                }

                // Either we don't have an array or we just moved to a hash
                // table.

                _table[key] = value;
            }

            public void Remove(object key)
            {
                if (_array != null)
                {
                    int index = FindIndex(key);

                    if (index != -1)
                    {
                        _array[index] = null;
                        _array[index + 1] = null;
                    }
                }
                else if (_table != null)
                {
                    _table.Remove(key);
                }
            }
        }
    }
}
