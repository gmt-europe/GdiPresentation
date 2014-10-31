using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace GdiPresentation.Demo
{
    [DisplayName("Plan board")]
    internal class PlanboardDemo : ElementDemo
    {
        private const int Days = 14;
        // private const int Rows = 800;
        private const int Rows = 50;
        private const int BarWidth = 120;

        private readonly Border[,] _cells = new Border[Days, Rows];
        private Point _dragStart;

        public override System.Windows.Forms.ScrollBars AllowedScrollBars
        {
            get { return System.Windows.Forms.ScrollBars.Both; }
        }

        public override Element BuildContent()
        {
            var grid = new Grid
            {
                Background = Brush.White
            };

            for (int i = 0; i < Days; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition(
                    new GridLength(GridUnitType.Pixel, 260)
                ));

                var dayLane = BuildDayLane(i);

                Grid.SetColumn(dayLane, i);

                grid.Children.Add(dayLane);
            }

            var container = new StaticPanel();

            container.Children.Add(grid);

            var topHeader = new Border
            {
                BorderThickness = new Thickness(2),
                BorderBrush = Brush.Blue,
                Background = Brush.White,
                Content = new TextBlock("Top header")
                {
                    Margin = new Thickness(2)
                }
            };

            StaticPanel.SetStaticAlignment(topHeader, StaticAlignment.Top);

            container.Children.Add(topHeader);

            var leftHeader = new Border
            {
                BorderThickness = new Thickness(2),
                BorderBrush = Brush.Blue,
                Background = Brush.White,
                Content = new TextBlock("Left header")
                {
                    Margin = new Thickness(2)
                }
            };

            StaticPanel.SetStaticAlignment(leftHeader, StaticAlignment.Left);

            container.Children.Add(leftHeader);

            var topLeftHeader = new Border
            {
                Background = Brush.White
            };

            StaticPanel.SetStaticAlignment(topLeftHeader, StaticAlignment.TopLeft);

            container.Children.Add(topLeftHeader);

            return container;
        }

        private Element BuildDayLane(int index)
        {
            var grid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition(),
                    new RowDefinition(new GridLength(GridUnitType.Star, 1))
                },
                Children =
                {
                    CreateHeader(index)
                }
            };

            var rows = new StackPanel
            {
                Orientation = Orientation.Vertical
            };

            Grid.SetRow(rows, 1);

            grid.Children.Add(rows);

            for (int i = 0; i < Rows; i++)
            {
                var container = new Border
                {
                    Content = BuildCell(index, i),
                    Margin = new Thickness(-100, 0)
                };

                if (i != Rows - 1)
                {
                    container.BorderThickness = new Thickness(0, 0, 0, 1);
                    container.BorderBrush = Brush.Silver;
                }

                rows.Children.Add(container);

                _cells[index, i] = container;
            }

            var border = new Border
            {
                Content = grid
            };

            if (index != Days - 1)
            {
                border.BorderBrush = Brush.Black;
                border.BorderThickness = new Thickness(0, 0, 1, 0);
            }

            return border;
        }

        private static VisualElement CreateHeader(int index)
        {
            var header = new VisualElement
            {
                Style = VisualStyle.ListViewHeader,
                Background = Brush.Transparent,
                Content = new TextBlock("Day " + (index + 1))
                {
                    VerticalAlignment = VerticalAlignment.Middle,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(3)
                }
            };

            header.MouseEnter += (s, e) => header.Mode = VisualMode.Hot;
            header.MouseLeave += (s, e) => header.Mode = VisualMode.Normal;
            header.MouseDown += (s, e) => header.Mode = VisualMode.Pressed;
            header.MouseUp += (s, e) => header.Mode = VisualMode.Normal;

            return header;
        }

        private Element BuildCell(int day, int row)
        {
            var draggable = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition(new GridLength(GridUnitType.Pixel, 3)),
                    new ColumnDefinition(new GridLength(GridUnitType.Star, 1)),
                    new ColumnDefinition(new GridLength(GridUnitType.Pixel, 3))
                },
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Background = Brush.Transparent,
                MinWidth = BarWidth,
                Margin = new Thickness(150, 0, 0, 0)
            };

            var content = new Border
            {
                Content = new TextBlock("This can be dragged")
                {
                    Margin = new Thickness(2)
                },
                Background = Brush.White,
                BorderBrush = Brush.Red,
                BorderThickness = new Thickness(1),
                Cursor = Cursor.SizeAll
            };

            draggable.MouseDown += draggable_MouseDown;
            draggable.MouseMove += draggable_MouseMove;

            Grid.SetColumnSpan(content, 3);

            var leftResizer = new Resizer(draggable, Resizer.Type.Left);
            var rightResizer = new Resizer(draggable, Resizer.Type.Right);

            Grid.SetColumn(rightResizer, 2);

            draggable.Children.Add(content);
            draggable.Children.Add(leftResizer);
            draggable.Children.Add(rightResizer);

            return draggable;
        }

        void draggable_MouseDown(object sender, MouseEventArgs e)
        {
            e.PreventBubble();

            var draggable = (Element)sender;

            draggable.Capture = true;
            _dragStart = e.GetPosition(draggable.Parent) - new Point(
                draggable.Margin.Left,
                draggable.Margin.Top
            );
        }

        void draggable_MouseMove(object sender, MouseEventArgs e)
        {
            e.PreventBubble();

            var draggable = (Element)sender;

            if (!draggable.Capture)
                return;

            var location = e.GetPosition(draggable.Parent);
            var offset = location - _dragStart;

            int left = Math.Max(0, Math.Min(offset.X, draggable.Parent.ActualWidth - draggable.ActualWidth));

            draggable.Margin = new Thickness(
                left,
                0,
                0,
                0
            );
        }

        private class Resizer : Element
        {
            private readonly Element _container;
            private readonly Type _type;
            private int _startWidth;
            private int _startOffset;
            private Point _dragStart;

            public Resizer(Element container, Type type)
            {
                _container = container;
                _type = type;

                Background = Brush.Transparent;
                Cursor = Cursor.SizeWE;
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                e.PreventBubble();

                Capture = true;

                _dragStart = e.GetPosition(_container.Parent);

                _startWidth = Math.Max(_container.ActualWidth, _container.MinWidth);
                _startOffset = _container.Margin.Left;
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                e.PreventBubble();

                if (!Capture)
                    return;

                var location = e.GetPosition(_container.Parent);
                var offset = location - _dragStart;

                if (_type == Type.Left)
                {
                    int left = Math.Max(
                        0,
                        Math.Min(
                            _startOffset + offset.X,
                            (_startOffset + _startWidth) - BarWidth
                        )
                    );

                    _container.MinWidth += _container.Margin.Left - left;
                    _container.Margin = new Thickness(left, 0, 0, 0);
                }
                else
                {
                    _container.MinWidth = Math.Max(
                        Math.Min(
                            _startWidth + offset.X,
                            _container.Parent.ActualWidth - _container.Margin.Left
                        ),
                        BarWidth
                    );
                }
            }

            public enum Type
            {
                Left,
                Right
            }
        }
    }
}
