using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GdiPresentation
{
    internal class LayoutManager
    {
        public ElementHost Host { get; private set; }

        public LayoutQueue<Element> PaintElementQueue { get; private set; }

        public LayoutQueue<Rect> PaintBoundsQueue { get; private set; }

        public LayoutManager(ElementHost host)
        {
            if (host == null)
                throw new ArgumentNullException("host");

            Host = host;
            PaintElementQueue = new PaintElementLayoutQueue(this);
            PaintBoundsQueue = new PaintBoundsLayoutQueue(this);
        }

        public void InvalidateMeasure(Element element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            for (var parent = element.Parent; parent != null && parent.IsMeasureValid; parent = parent.Parent)
            {
                parent.InvalidateMeasureInternal();
            }
        }

        public void InvalidateArrange(Element element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            for (var parent = element.Parent; parent != null && parent.IsArrangeValid; parent = parent.Parent)
            {
                parent.InvalidateArrangeInternal();
            }
        }

        private class PaintElementLayoutQueue : LayoutQueue<Element>
        {
            private readonly ElementControl _control;

            public PaintElementLayoutQueue(LayoutManager layoutManager)
            {
                _control = layoutManager.Host.Control;
            }

            protected override void ItemAdded()
            {
                _control.QueueInvalidate();
            }
        }

        private class PaintBoundsLayoutQueue : LayoutQueue<Rect>
        {
            private readonly ElementControl _control;

            public PaintBoundsLayoutQueue(LayoutManager layoutManager)
            {
                _control = layoutManager.Host.Control;
            }

            protected override void ItemAdded()
            {
                _control.QueueInvalidate();
            }
        }
    }
}
