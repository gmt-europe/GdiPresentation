using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace GdiPresentation.Demo
{
    [DisplayName("Visibility")]
    internal class ChangeVisibilityDemo : ElementDemo
    {
        private Border _border1;
        private Border _border2;

        public override System.Windows.Forms.ScrollBars AllowedScrollBars
        {
            get { return System.Windows.Forms.ScrollBars.None; }
        }

        public override Element BuildContent()
        {
            var visibleButton = new Border
            {
                Content = new TextBlock("Visible"),
                Background = Brush.White,
                BorderThickness = new Thickness(1),
                BorderBrush = Brush.Blue,
                Margin = new Thickness(3)
            };

            visibleButton.MouseUp += (s, e) => SetState(Visibility.Visible);

            var hiddenButton = new Border
            {
                Content = new TextBlock("Hidden"),
                Background = Brush.White,
                BorderThickness = new Thickness(1),
                BorderBrush = Brush.Blue,
                Margin = new Thickness(3)
            };

            hiddenButton.MouseUp += (s, e) => SetState(Visibility.Hidden);

            var collapsedButton = new Border
            {
                Content = new TextBlock("Collapsed"),
                Background = Brush.White,
                BorderThickness = new Thickness(1),
                BorderBrush = Brush.Blue,
                Margin = new Thickness(3)
            };

            collapsedButton.MouseUp += (s, e) => SetState(Visibility.Collapsed);

            _border1 = new Border
            {
                Background = Brush.Yellow,
                Content = new TextBlock("Collapsing element"),
                Margin = new Thickness(3)
            };

            _border2 = new Border
            {
                Background = Brush.Yellow,
                Content = new TextBlock("Collapsing element"),
                Margin = new Thickness(3)
            };

            return new StackPanel
            {
                Orientation = Orientation.Vertical,
                Background = Brush.White,
                Children =
                {
                    new StackPanel
                    {
                        Children =
                        {
                            visibleButton,
                            hiddenButton,
                            collapsedButton
                        }
                    },
                    new Border
                    {
                        Background = Brush.Blue,
                        Margin = new Thickness(3),
                        MinHeight = 10
                    },
                    new StackPanel
                    {
                        Children =
                        {
                            _border1,
                            new Border
                            {
                                Background = Brush.Yellow,
                                Content = new TextBlock("Spacer element"),
                                Margin = new Thickness(3)
                            }
                        }
                    },
                    new Border
                    {
                        Background = Brush.Blue,
                        Margin = new Thickness(3),
                        MinHeight = 10
                    },
                    _border2
                }
            };
        }

        private void SetState(Visibility visibility)
        {
            _border1.Visibility = visibility;
            _border2.Visibility = visibility;
        }
    }
}
