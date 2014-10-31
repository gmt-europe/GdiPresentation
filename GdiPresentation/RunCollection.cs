using System;
using System.Collections.Generic;
using System.Text;

namespace GdiPresentation
{
    public class RunCollection : Collection<Run>
    {
        private readonly TextBlock _owner;

        public event EventHandler CollectionChanged;

        protected virtual void OnCollectionChanged(EventArgs e)
        {
            var ev = CollectionChanged;

            if (ev != null)
                ev(this, e);
        }

        public RunCollection(TextBlock owner)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");

            _owner = owner;
        }

        protected override void ClearItems()
        {
            for (int i = 0, count = Count; i < count; i++)
            {
                this[i].Parent = null;
            }

            base.ClearItems();

            OnCollectionChanged(EventArgs.Empty);
        }

        protected override void InsertItem(int index, Run item)
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

        protected override void SetItem(int index, Run item)
        {
            this[index].Parent = null;

            base.SetItem(index, item);

            item.Parent = _owner;

            OnCollectionChanged(EventArgs.Empty);
        }
    }
}
