using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace GdiPresentation.Demo
{
    [DisplayName("Keyboard focus")]
    internal class KeyboardFocusDemo : ElementDemo
    {
        private TextBlock _currentLabel;
        private Element _current;
        private StackPanel _root;

        public override System.Windows.Forms.ScrollBars AllowedScrollBars
        {
            get { return System.Windows.Forms.ScrollBars.None; }
        }

        public override Element BuildContent()
        {
            var nextButton = new Border
            {
                Content = new TextBlock("Next"),
                Background = Brush.White,
                BorderBrush = Brush.Red,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(2)
            };

            nextButton.MouseUp += nextButton_MouseUp;

            var previousButton = new Border
            {
                Content = new TextBlock("Previous"),
                Background = Brush.White,
                BorderBrush = Brush.Red,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(2)
            };

            previousButton.MouseUp += previousButton_MouseUp;

            _currentLabel = new TextBlock("????");

            var topRow = new StackPanel
            {
                Children =
                {
                    previousButton,
                    nextButton,
                    new Border
                    {
                        Background = Brush.White,
                        BorderBrush = Brush.Red,
                        BorderThickness = new Thickness(1),
                        Content = _currentLabel,
                        Margin = new Thickness(2)
                    }
                }
            };

            _root = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Focusable = true,
                Children =
                {
                    topRow,
                    new StackPanel
                    {
                        Children =
                        {
                            CreateBordered("Row 1 child 1"),
                            CreateBordered("Row 1 child 2"),
                            CreateBordered("Row 1 child 3")
                        }
                    },
                    new StackPanel
                    {
                        Children =
                        {
                            BuildInner(
                                CreateBordered("Row 2 child 1 cell 1"),
                                CreateBordered("Row 2 child 1 cell 2", true),
                                CreateBordered("Row 2 child 1 cell 3")
                            ),
                            BuildInner(
                                CreateBordered("Row 2 child 2 cell 1", true),
                                CreateBordered("Row 2 child 2 cell 2")
                            ),
                            new StackPanel
                            {
                                Orientation = Orientation.Vertical,
                                Children =
                                {
                                    CreateBordered("Row 2 child 3 cell 1"),
                                }
                            },
                        }
                    }
                }
            };

            _root.GotFocus += (s, e) => _root.Background = Brush.Blue;
            _root.LostFocus += (s, e) => _root.Background = Brush.White;

            _current = _root;

            return _root;
        }

        private StackPanel BuildInner(params Element[] children)
        {
            var innerStackPanel = new StackPanel
            {
                Focusable = true,
                Orientation = Orientation.Vertical,
                Tag = "Hi"
            };

            innerStackPanel.Children.AddRange(children);

            innerStackPanel.GotFocus += (s, e) => innerStackPanel.Background = Brush.Blue;
            innerStackPanel.LostFocus += (s, e) => innerStackPanel.Background = Brush.White;

            return innerStackPanel;
        }

        private Element CreateBordered(string label)
        {
            return CreateBordered(label, false);
        }

        private Element CreateBordered(string label, bool textBox)
        {
            Element content;

            if (textBox)
                content = new TextBoxInput { Control = { Text = label } };
            else
                content = new TextBlock(label);

            var border = new Border
            {
                Background = Brush.White,
                BorderThickness = new Thickness(2),
                BorderBrush = Brush.Black,
                Content = content,
                Focusable = !textBox,
                Margin = new Thickness(2),
                Tag = label,
                Padding = new Thickness(3)
            };

            border.GotFocus += border_GotFocus;
            border.LostFocus += border_LostFocus;
            border.IsKeyboardFocusWithinChanged += border_IsKeyboardFocusWithinChanged;

            return border;
        }

        void border_IsKeyboardFocusWithinChanged(object sender, EventArgs e)
        {
            Brush brush;

            if (((Border)sender).IsKeyboardFocusWithin)
                brush = Brush.Lime;
            else
                brush = Brush.Black;

            ((Border)sender).BorderBrush = brush;
        }

        void border_LostFocus(object sender, EventArgs e)
        {
            ((Border)sender).Background = Brush.White;
        }

        void border_GotFocus(object sender, EventArgs e)
        {
            ((Border)sender).Background = Brush.Yellow;
        }

        void previousButton_MouseUp(object sender, MouseEventArgs e)
        {
            e.PreventBubble();

            _current = _current.FindNextElement(true, true, false);

            UpdateFromCurrent();
        }

        void nextButton_MouseUp(object sender, MouseEventArgs e)
        {
            e.PreventBubble();

            _current = _current.FindNextElement(true, true, true);

            UpdateFromCurrent();
        }

        private void UpdateFromCurrent()
        {
            _currentLabel.Runs.Clear();
            _currentLabel.Runs.Add(new Run((string)_current.Tag));
        }
    }
}
