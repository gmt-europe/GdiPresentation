using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GdiPresentation
{
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    public class Collection<T> : System.Collections.ObjectModel.Collection<T>
    {
        public Collection()
        {
        }

        public Collection(IList<T> list)
            : base(list)
        {
        }

        public void AddRange(IList<T> list)
        {
            foreach (var item in list)
                Add(item);
        }
    }
}
