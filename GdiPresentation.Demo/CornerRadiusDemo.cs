using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace GdiPresentation.Demo
{
    [DisplayName("Corner radius")]
    internal class CornerRadiusDemo : ElementDemo
    {
        public override Element BuildContent()
        {
            var result = new StackPanel
            {
                Margin = new Thickness(20),
                Orientation = Orientation.Vertical,
                Children =
                {
                    // Simple borders.

                    new Border
                    {
                        MinHeight = 50, MinWidth = 50, Margin = new Thickness(10),
                        BorderThickness = new Thickness(1),
                        BorderBrush = Brush.Blue,
                        CornerRadius = new CornerRadius(2)
                    },
                    new Border
                    {
                        MinHeight = 50, MinWidth = 50, Margin = new Thickness(10),
                        BorderThickness = new Thickness(1),
                        BorderBrush = Brush.Blue,
                        CornerRadius = new CornerRadius(5)
                    },
                    new Border
                    {
                        MinHeight = 50, MinWidth = 50, Margin = new Thickness(10),
                        BorderThickness = new Thickness(5),
                        BorderBrush = Brush.Blue,
                        CornerRadius = new CornerRadius(2)
                    },
                    new Border
                    {
                        MinHeight = 50, MinWidth = 50, Margin = new Thickness(10),
                        BorderThickness = new Thickness(5),
                        BorderBrush = Brush.Blue,
                        CornerRadius = new CornerRadius(5)
                    },

                    // Simple fills.

                    new Border
                    {
                        MinHeight = 50, MinWidth = 50, Margin = new Thickness(10),
                        Background = Brush.Red,
                        CornerRadius = new CornerRadius(2)
                    },
                    new Border
                    {
                        MinHeight = 50, MinWidth = 50, Margin = new Thickness(10),
                        Background = Brush.Red,
                        CornerRadius = new CornerRadius(5)
                    },

                    // Irregular corners.

                    new Border
                    {
                        MinHeight = 50, MinWidth = 50, Margin = new Thickness(10),
                        BorderThickness = new Thickness(1),
                        BorderBrush = Brush.Blue,
                        CornerRadius = new CornerRadius(2, 4, 6, 8)
                    },
                    new Border
                    {
                        MinHeight = 50, MinWidth = 50, Margin = new Thickness(10),
                        BorderThickness = new Thickness(1),
                        BorderBrush = Brush.Blue,
                        CornerRadius = new CornerRadius(5, 10, 15, 20)
                    },
                    new Border
                    {
                        MinHeight = 50, MinWidth = 50, Margin = new Thickness(10),
                        BorderThickness = new Thickness(5),
                        BorderBrush = Brush.Blue,
                        CornerRadius = new CornerRadius(2, 4, 6, 8)
                    },
                    new Border
                    {
                        MinHeight = 50, MinWidth = 50, Margin = new Thickness(10),
                        BorderThickness = new Thickness(5),
                        BorderBrush = Brush.Blue,
                        CornerRadius = new CornerRadius(5, 10, 15, 20)
                    },

                }
            };

            return new Border
            {
                Margin = new Thickness(50),
                Content = new Border
                {
                    BorderBrush = Brush.Blue,
                    BorderThickness = new Thickness(4),
                    Content = result
                }
            };
        }

        public override System.Windows.Forms.ScrollBars AllowedScrollBars
        {
            get { return System.Windows.Forms.ScrollBars.Both; }
        }
    }
}
