using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace GdiPresentation.Demo
{
    [DisplayName("Tree Expander")]
    internal class TreeExpanderDemo : ElementDemo
    {
        public override Element BuildContent()
        {
            var container = new StackPanel
            {
                Orientation = Orientation.Vertical
            };

            for (int i = 0; i < 5; i++)
            {
                container.Children.Add(BuildExpander(1));
            }

            return new Border
            {
                Margin = new Thickness(50),
                BorderThickness = new Thickness(1),
                BorderBrush = Brush.Red,
                Content = container
            };
        }

        private Element BuildExpander(int level)
        {
            var result = new TreeExpander
            {
                IsExpanded = true,
                Header = new Border
                {
                    BorderThickness = new Thickness(1),
                    BorderBrush = Brush.Black,
                    Background = Brush.White,
                    Content = new TextBlock("This is an expander")
                    {
                        Margin = new Thickness(3)
                    }
                }
            };

            if (level == 1)
                result.ContainerMargin = new Thickness(0, 2, 0, 2);

            for (int i = level; i < 4; i++)
            {
                result.Children.Add(BuildExpander(level + 1));
            }

            return result;
        }

        public override System.Windows.Forms.ScrollBars AllowedScrollBars
        {
            get { return System.Windows.Forms.ScrollBars.Both; }
        }
    }
}
