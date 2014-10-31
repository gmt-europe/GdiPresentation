using System;
using System.Collections.Generic;
using System.Text;

namespace GdiPresentation
{
    public abstract class DefinitionCollection<T> : Collection<T>
        where T : DefinitionBase
    {
        private readonly Grid _owner;

        public event EventHandler CollectionChanged;

        protected virtual void OnCollectionChanged(EventArgs e)
        {
            var ev = CollectionChanged;

            if (ev != null)
                ev(this, e);
        }

        internal DefinitionCollection(Grid owner)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");

            _owner = owner;
        }

        protected override void ClearItems()
        {
            foreach (var item in this)
            {
                item.Parent = null;
            }

            base.ClearItems();

            OnCollectionChanged(EventArgs.Empty);
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);

            item.Parent = _owner;

            OnCollectionChanged(EventArgs.Empty);
        }

        protected override void RemoveItem(int index)
        {
            this[index].Parent = null;

            base.RemoveItem(index);

            OnCollectionChanged(EventArgs.Empty);
        }

        protected override void SetItem(int index, T item)
        {
            this[index].Parent = null;

            base.SetItem(index, item);

            item.Parent = _owner;

            OnCollectionChanged(EventArgs.Empty);
        }
    }

    public class ColumnDefinitionCollection : DefinitionCollection<ColumnDefinition>
    {
        public ColumnDefinitionCollection(Grid owner)
            : base(owner)
        {
        }
    }

    public class RowDefinitionCollection : DefinitionCollection<RowDefinition>
    {
        public RowDefinitionCollection(Grid owner)
            : base(owner)
        {
        }
    }
}
