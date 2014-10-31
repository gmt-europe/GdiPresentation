using System;
using System.Collections.Generic;
using System.Text;

namespace GdiPresentation
{
    public class ElementCollection : Collection<Element>
    {
        private readonly Element _owner;

        public event EventHandler CollectionChanged;

        protected virtual void OnCollectionChanged(EventArgs e)
        {
            var ev = CollectionChanged;

            if (ev != null)
                ev(this, e);
        }

        internal ElementCollection(Element owner)
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

        protected override void InsertItem(int index, Element item)
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

        protected override void SetItem(int index, Element item)
        {
            this[index].Parent = null;

            base.SetItem(index, item);

            item.Parent = _owner;

            OnCollectionChanged(EventArgs.Empty);
        }
    }
}
