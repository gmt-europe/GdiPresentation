using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace GdiPresentation.Demo
{
    [DisplayName("Basic rendering")]
    internal class BasicRenderingDemo : ElementDemo
    {
        public override System.Windows.Forms.ScrollBars AllowedScrollBars
        {
            get { return System.Windows.Forms.ScrollBars.Vertical; }
        }

        public override Element BuildContent()
        {
            var result = new StackPanel
            {
                Orientation = Orientation.Vertical
            };

            for (int i = 0; i < 10; i++)
            {
                var border = new Border
                {
                    Padding = new Thickness(20),
                    Margin = new Thickness(10),
                    Background = Brush.White,
                    BorderBrush = Brush.Black,
                    BorderThickness = new Thickness(1),
                    Content = new TextBlock("Some text as content. Some text as content. Some text as content. Some text as content. Some text as content. Some text as content. Some text as content. Some text as content. Some text as content. Some text as content. Some text as content.")
                    {
                        Wrap = true
                    }
                };

                border.MouseEnter += border_MouseEnter;
                border.MouseDown += border_MouseDown;
                border.MouseUp += border_MouseUp;
                border.MouseMove += border_MouseMove;
                border.MouseLeave += border_MouseLeave;

                result.Children.Add(border);
            }

            return result;
        }

        void border_MouseUp(object sender, MouseEventArgs e)
        {
            var border = (Border)sender;

            if (border.Capture)
                border.Background =
                    border.IsMouseDirectlyOver
                    ? Brush.Red
                    : Brush.White;
        }

        void border_MouseDown(object sender, MouseEventArgs e)
        {
            var border = (Border)sender;

            border.Background = Brush.Yellow;
            border.Capture = true;
        }

        void border_MouseEnter(object sender, EventArgs e)
        {
            var border = (Border)sender;

            border.Background = Brush.Red;
        }

        void border_MouseMove(object sender, MouseEventArgs e)
        {
            var border = (Border)sender;

            if (border.Capture)
                border.Background =
                    border.IsMouseDirectlyOver
                    ? Brush.Yellow
                    : Brush.Green;
        }

        void border_MouseLeave(object sender, EventArgs e)
        {
            var border = (Border)sender;

            if (!border.Capture)
                border.Background = Brush.White;
        }
    }
}
