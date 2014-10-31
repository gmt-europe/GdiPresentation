using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GdiPresentation
{
    internal class CollectionDebugView<T>
    {
        private readonly Collection<T> _container;

        public CollectionDebugView(Collection<T> container)
        {
            _container = container;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get { return new List<T>(_container).ToArray(); }
        }
    }
}
