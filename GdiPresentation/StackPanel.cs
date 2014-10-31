using System;
using System.Collections.Generic;
using System.Text;

namespace GdiPresentation
{
    public class StackPanel : ContainerElement
    {
        private Orientation _orientation;

        public Orientation Orientation
        {
            get { return _orientation; }
            set
            {
                if (_orientation != value)
                {
                    _orientation = value;
                    InvalidateMeasure();
                }
            }
        }

        protected override Size MeasureOverride(Size desiredSize)
        {
            int sum = 0;
            int max = 0;

            var size = Orientation == Orientation.Vertical
                ? new Size(desiredSize.Width, int.MaxValue)
                : new Size(int.MaxValue, desiredSize.Height);

            for (int i = 0, count = Children.Count; i < count; i++)
            {
                var item = Children[i];

                item.Measure(size);

                if (Orientation == Orientation.Vertical)
                {
                    sum += item.DesiredSize.Height;
                    max = Math.Max(max, item.DesiredSize.Width);
                }
                else
                {
                    sum += item.DesiredSize.Width;
                    max = Math.Max(max, item.DesiredSize.Height);
                }
            }

            if (Orientation == Orientation.Vertical)
                return new Size(max, sum);
            else
                return new Size(sum, max);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            int sum = 0;

            for (int i = 0, count = Children.Count; i < count; i++)
            {
                var item = Children[i];

                Rect bounds;

                if (Orientation == Orientation.Vertical)
                {
                    bounds = new Rect(
                        0,
                        sum,
                        finalSize.Width,
                        Math.Min(item.DesiredSize.Height, finalSize.Height - sum)
                    );
                }
                else
                {
                    bounds = new Rect(
                        sum,
                        0,
                        Math.Min(item.DesiredSize.Width, finalSize.Width - sum),
                        finalSize.Height
                    );
                }

                item.Arrange(bounds);

                if (item.Visibility != Visibility.Collapsed)
                {
                    if (Orientation == Orientation.Vertical)
                        sum += item.ActualHeight + item.Margin.Vertical;
                    else
                        sum += item.ActualWidth + item.Margin.Horizontal;
                }
            }

            if (Orientation == Orientation.Vertical)
                return new Size(finalSize.Width, Math.Max(finalSize.Height, sum));
            else
                return new Size(Math.Max(finalSize.Width, sum), finalSize.Height);
        }
    }
}
