using System;
using System.Collections.Generic;
using System.Text;

namespace GdiPresentation
{
    public class Canvas : ContainerElement
    {
        private static readonly NamedObject LeftProperty = new NamedObject("Canvas.Left");
        private static readonly NamedObject TopProperty = new NamedObject("Canvas.Top");

        public static int GetLeft(Element element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return ((int?)element.GetAttachedValue(LeftProperty)).GetValueOrDefault(0);
        }

        public static void SetLeft(Element element, int value)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            element.SetAttachedValue(LeftProperty, value == 0 ? null : (int?)value);

            AttachedValueChanged(element);
        }

        public static int GetTop(Element element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return ((int?)element.GetAttachedValue(TopProperty)).GetValueOrDefault(0);
        }

        public static void SetTop(Element element, int value)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            element.SetAttachedValue(TopProperty, value == 0 ? null : (int?)value);

            AttachedValueChanged(element);
        }

        private static void AttachedValueChanged(Element element)
        {
            var parent = element.Parent as Canvas;

            if (parent != null)
                parent.InvalidateMeasure();
        }

        protected override Size MeasureOverride(Size desiredSize)
        {
            int maxX = 0;
            int maxY = 0;

            for (int i = 0, count = Children.Count; i < count; i++)
            {
                var item = Children[i];

                int left = GetLeft(item);
                int top = GetTop(item);

                item.Measure(new Size(
                    desiredSize.Width - left,
                    desiredSize.Height - top
                ));

                maxX = Math.Max(maxX, left + item.DesiredSize.Width);
                maxY = Math.Max(maxY, top + item.DesiredSize.Height);
            }

            return new Size(maxX, maxY);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            int maxX = 0;
            int maxY = 0;

            for (int i = 0, count = Children.Count; i < count; i++)
            {
                var item = Children[i];

                int left = GetLeft(item);
                int top = GetTop(item);

                item.Arrange(new Rect(
                    left,
                    top,
                    item.DesiredSize.Width,
                    item.DesiredSize.Height
                ));

                if (item.Visibility != Visibility.Collapsed)
                {
                    maxX = Math.Max(maxX, left + item.ActualWidth + item.Margin.Horizontal);
                    maxY = Math.Max(maxY, top + item.ActualHeight + item.Margin.Vertical);
                }
            }

            return new Size(
                Math.Max(finalSize.Width, maxX),
                Math.Max(finalSize.Height, maxY)
            );
        }
    }
}
