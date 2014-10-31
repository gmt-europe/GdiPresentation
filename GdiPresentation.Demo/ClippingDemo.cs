using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace GdiPresentation.Demo
{
    [DisplayName("Clipping")]
    internal class ClippingDemo : ElementDemo
    {
        private Point _dragStart;

        public override System.Windows.Forms.ScrollBars AllowedScrollBars
        {
            get { return System.Windows.Forms.ScrollBars.None; }
        }

        public override Element BuildContent()
        {
            var container = new NonStretchingBorder();

            var draggable = new Border
            {
                Content = new TextBlock("This can be dragged"),
                Background = Brush.White,
                BorderBrush = Brush.Red,
                BorderThickness = new Thickness(1),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(50, 50, 0, 0)
            };

            draggable.MouseDown += _draggable_MouseDown;
            draggable.MouseMove += _draggable_MouseMove;
            draggable.MouseUp += _draggable_MouseUp;

            container.Content = draggable;

            return new Border
            {
                Background = Brush.White,
                Content = new Border
                {
                    BorderThickness = new Thickness(5),
                    BorderBrush = Brush.Red,
                    Margin = new Thickness(40),
                    Content = container
                }
            };
        }

        void _draggable_MouseDown(object sender, MouseEventArgs e)
        {
            var draggable = (Border)sender;

            draggable.Capture = true;
            _dragStart = e.GetPosition(draggable.Parent);
        }

        void _draggable_MouseMove(object sender, MouseEventArgs e)
        {
            var draggable = (Border)sender;

            if (!draggable.Capture)
                return;

            var location = e.GetPosition(draggable.Parent);
            var offset = location - _dragStart;

            draggable.Margin = new Thickness(
                draggable.Margin.Left + offset.X,
                draggable.Margin.Top + offset.Y,
                0,
                0
            );

            _dragStart = location;
        }

        void _draggable_MouseUp(object sender, MouseEventArgs e)
        {
        }

        private class NonStretchingBorder : Border
        {
            protected override Size MeasureOverride(Size desiredSize)
            {
                base.MeasureOverride(desiredSize);

                return Size.Empty;
            }
        }
    }
}
