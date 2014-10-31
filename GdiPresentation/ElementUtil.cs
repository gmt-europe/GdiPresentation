using System;
using System.Collections.Generic;
using System.Text;

namespace GdiPresentation
{
    public static class ElementUtil
    {
        public static Element FindAtLocation(Element element, Point location)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            for (int i = 0, count = element.GetChildrenCount(); i < count; i++)
            {
                var child = element.GetChild(i);

                child = FindAtLocation(child, location - child.Offset);

                if (child != null)
                    return child;
            }

            if (
                location.X >= 0 &&
                location.Y >= 0 &&
                location.X <= element.ActualWidth &&
                location.Y <= element.ActualHeight
            )
                return element;

            return null;
        }

        public static Point GetScrollOffset(Element element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            if (element.LayoutManager == null)
                return new Point(0, 0);

            return element.LayoutManager.Host.ScrollOffset;
        }

        public static ElementControl FindHost(Element element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            if (element.LayoutManager != null)
                return element.LayoutManager.Host.Control;

            return null;
        }
    }
}
