using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace GdiPresentation.Demo
{
    [DisplayName("Adjust for scrollbars")]
    internal class AdjustForScrollBarsDemo : ElementDemo
    {
        public override Element BuildContent()
        {
            return new Border
            {
                Background = Brush.Red,
                Content = new Border
                {
                    Width = 500,
                    Height = 500,
                    Background = Brush.Blue,
                    Content = new Border
                    {
                        Margin = new Thickness(20),
                        BorderThickness = new Thickness(1),
                        BorderBrush = Brush.Yellow
                    }
                }
            };
        }

        public override System.Windows.Forms.ScrollBars AllowedScrollBars
        {
            get { return System.Windows.Forms.ScrollBars.Both; }
        }

        public override ScrollBarVisibility ShowHorizontalScrollBar
        {
            get { return ScrollBarVisibility.Visible; }
        }
    }
}
