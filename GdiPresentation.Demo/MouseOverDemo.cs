using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

namespace GdiPresentation.Demo
{
    [DisplayName("Mouse over")]
    internal class MouseOverDemo : ElementDemo
    {
        public override Element BuildContent()
        {
            var outer = new Border
            {
                Margin = new Thickness(50),
                Background = Brush.Blue,
                BorderThickness = new Thickness(4)
            };

            outer.IsMouseDirectlyOverChanged += outer_IsMouseDirectlyOverChanged;
            outer.MouseEnter += (s, e) => outer.BorderBrush = Brush.Fuchsia;
            outer.MouseLeave += (s, e) => outer.BorderBrush = null;

            var innerContainer = new Border
            {
                Margin = new Thickness(50)
            };

            innerContainer.MouseEnter += (s, e) => innerContainer.Background = Brush.Purple;

            var inner = new Border
            {
                Margin = new Thickness(50),
                Background = Brush.Red,
                Content = innerContainer,
                BorderThickness = new Thickness(4)
            };

            inner.IsMouseDirectlyOverChanged += inner_IsMouseDirectlyOverChanged;
            inner.MouseEnter += (s, e) => inner.BorderBrush = Brush.Fuchsia;
            inner.MouseLeave += (s, e) => inner.BorderBrush = null;

            var outerContainer = new Border
            {
                Margin = new Thickness(50),
                Content = inner
            };

            outerContainer.MouseEnter += (s, e) => outerContainer.Background = Brush.Purple;

            outer.Content = outerContainer;

            return outer;
        }

        void outer_IsMouseDirectlyOverChanged(object sender, EventArgs e)
        {
            ((Border)sender).Background = ((Border)sender).IsMouseDirectlyOver ? Brush.Green : Brush.Blue;
        }

        void inner_IsMouseDirectlyOverChanged(object sender, EventArgs e)
        {
            ((Border)sender).Background = ((Border)sender).IsMouseDirectlyOver ? Brush.Green : Brush.Red;
        }

        public override ScrollBars AllowedScrollBars
        {
            get { return ScrollBars.None; }
        }
    }
}
