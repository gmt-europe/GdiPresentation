using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace GdiPresentation.Demo
{
    [DisplayName("Dragging")]
    internal class DraggingDemo : ElementDemo
    {
        private Point _dragStart;
        private Canvas _canvas;

        public override System.Windows.Forms.ScrollBars AllowedScrollBars
        {
            get { return System.Windows.Forms.ScrollBars.Both; }
        }

        public override Element BuildContent()
        {
            _canvas = new Canvas
            {
                Background = Brush.White
            };

            for (int i = 0; i < 20; i++)
            {
                var draggable = new Border
                {
                    Content = new TextBlock("This can be dragged"),
                    Background = Brush.White,
                    BorderBrush = Brush.Red,
                    BorderThickness = new Thickness(1),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                };

                Canvas.SetLeft(draggable, i * 20);
                Canvas.SetTop(draggable, i * 20);

                draggable.MouseDown += _draggable_MouseDown;
                draggable.MouseMove += _draggable_MouseMove;

                _canvas.Children.Add(draggable);
            }

            return _canvas;
        }

        void _draggable_MouseDown(object sender, MouseEventArgs e)
        {
            var draggable = (Border)sender;

            _canvas.Children.Remove(draggable);
            _canvas.Children.Add(draggable);

            draggable.Capture = true;
            _dragStart = e.GetPosition(draggable.Parent);

            e.PreventBubble();
        }

        void _draggable_MouseMove(object sender, MouseEventArgs e)
        {
            var draggable = (Border)sender;

            if (!draggable.Capture)
                return;

            var location = e.GetPosition(draggable.Parent);
            var offset = location - _dragStart;

            Canvas.SetLeft(
                draggable,
                Canvas.GetLeft(draggable) + offset.X
            );
            Canvas.SetTop(
                draggable,
                Canvas.GetTop(draggable) + offset.Y
            );

            _dragStart = location;
        }
    }
}
