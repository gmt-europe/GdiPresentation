using System;
using System.Collections.Generic;
using System.Text;

namespace GdiPresentation
{
    public class StaticPanel : ContainerElement
    {
        private static readonly NamedObject StaticAlignmentProperty = new NamedObject("StaticPanel.StaticAlignment");

        internal int LeftOffset { get; private set; }
        internal int TopOffset { get; private set; }

        protected override Size MeasureOverride(Size desiredSize)
        {
            VerifyParent();

            LeftOffset = 0;
            TopOffset = 0;

            for (int i = 0, count = Children.Count; i < count; i++)
            {
                var item = Children[i];

                var alignment = GetStaticAlignment(item);

                if (alignment == StaticAlignment.Normal)
                    continue;

                item.Measure(desiredSize);

                if (alignment == StaticAlignment.Left || alignment == StaticAlignment.TopLeft)
                    LeftOffset = Math.Max(LeftOffset, item.DesiredSize.Width);
                if (alignment == StaticAlignment.Top || alignment == StaticAlignment.TopLeft)
                    TopOffset = Math.Max(TopOffset, item.DesiredSize.Height);
            }

            var contentSize = new Size(
                desiredSize.Width - LeftOffset,
                desiredSize.Height - TopOffset
            );

            int contentWidth = 0;
            int contentHeight = 0;

            for (int i = 0, count = Children.Count; i < count; i++)
            {
                var item = Children[i];

                var alignment = GetStaticAlignment(item);

                if (alignment == StaticAlignment.Normal)
                    item.Measure(contentSize);

                if (
                    alignment == StaticAlignment.Top ||
                    alignment == StaticAlignment.Normal
                )
                    contentWidth = Math.Max(contentWidth, item.DesiredSize.Width);
                if (
                    alignment == StaticAlignment.Left ||
                    alignment == StaticAlignment.Normal
                )
                    contentHeight = Math.Max(contentHeight, item.DesiredSize.Height);
            }

            return new Size(
                LeftOffset + contentWidth,
                TopOffset + contentHeight
            );
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            VerifyParent();

            var displayOffset = LayoutManager.Host.Control.DisplayRectangle.Location;

            for (int i = 0, count = Children.Count; i < count; i++)
            {
                var item = Children[i];

                switch (GetStaticAlignment(item))
                {
                    case StaticAlignment.Left:
                        item.Arrange(new Rect(-displayOffset.X, TopOffset, item.DesiredSize.Width, finalSize.Height - TopOffset));
                        break;

                    case StaticAlignment.Top:
                        item.Arrange(new Rect(LeftOffset, -displayOffset.Y, finalSize.Width - LeftOffset, item.DesiredSize.Height));
                        break;

                    case StaticAlignment.TopLeft:
                        item.Arrange(new Rect(-displayOffset.X, -displayOffset.Y, LeftOffset, TopOffset));
                        break;

                    default:
                        item.Arrange(new Rect(LeftOffset, TopOffset, finalSize.Width - LeftOffset, finalSize.Height - TopOffset));
                        break;
                }
            }

            return finalSize;
        }

        private void VerifyParent()
        {
            if (!(Parent is ElementHost))
                throw new InvalidOperationException("Parent is invalid");
        }

        public static StaticAlignment GetStaticAlignment(Element element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return ((StaticAlignment?)element.GetAttachedValue(StaticAlignmentProperty)).GetValueOrDefault(StaticAlignment.Normal);
        }

        public static void SetStaticAlignment(Element element, StaticAlignment value)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            element.SetAttachedValue(StaticAlignmentProperty, value);
        }

        internal void ApplyScroll(int xDelta, int yDelta)
        {
            for (int i = 0, count = Children.Count; i < count; i++)
            {
                var item = Children[i];

                // We need to reposition the headers. When the offset
                // of a header changes because of the scroll (see
                // if below), we force an arrange of the element but
                // don't paint it. We don't need to paint it, because
                // the window scrolling ensures that the bits stay
                // valid.

                var alignment = StaticPanel.GetStaticAlignment(item);

                if (
                    alignment == StaticAlignment.Normal ||
                    (alignment == StaticAlignment.Left && xDelta == 0) ||
                    (alignment == StaticAlignment.Top && yDelta == 0)
                )
                    continue;

                var finalRect = item.PreviousFinalRect;

                item.InvalidateArrangeInternal();

                // Calculate the new bounds which in effect ensures
                // that nothing changes for the panel.

                item.Arrange(
                    new Rect(
                        finalRect.Left + (alignment == StaticAlignment.Left ? xDelta : 0),
                        finalRect.Top + (alignment == StaticAlignment.Top ? yDelta : 0),
                        finalRect.Width,
                        finalRect.Height
                    ),
                    false /* invalidatePaint */
                );
            }
        }

        internal override bool IsMouseOverCore(MouseEventArgs e, Element child)
        {
            if (!base.IsMouseOverCore(e, child))
                return false;

            var location = e.GetPosition(this);
            var displayRectangleLocation = LayoutManager.Host.Control.DisplayRectangle.Location;

            location = new Point(
                location.X + displayRectangleLocation.X,
                location.Y + displayRectangleLocation.Y
            );

            switch (GetStaticAlignment(child))
            {
                case StaticAlignment.Left:
                    return location.Y >= TopOffset;

                case StaticAlignment.Top:
                    return location.X >= LeftOffset;

                case StaticAlignment.Normal:
                    return
                        location.X >= LeftOffset &&
                        location.Y >= TopOffset;

                default:
                    return true;
            }
        }
    }
}
