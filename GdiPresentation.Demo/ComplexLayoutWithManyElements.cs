using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace GdiPresentation.Demo
{
    [DisplayName("Complex layout")]
    internal class ComplexLayoutWithManyElements : ElementDemo
    {
        public override System.Windows.Forms.ScrollBars AllowedScrollBars
        {
            get { return System.Windows.Forms.ScrollBars.Vertical; }
        }

        public override Element BuildContent()
        {
            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Children =
                {
                    new MyButton
                    {
                        Padding = new Thickness(20),
                        Margin = new Thickness(10)
                    },
                    new Border
                    {
                        BorderBrush = Brush.Red,
                        BorderThickness = new Thickness(1),
                        Background = new GradientBrush(Color.Red, Color.Green, 90),
                        Margin = new Thickness(1),
                        Padding = new Thickness(10),
                        MinWidth = 300,
                        Content = new TextBlock("Border with gradient")
                    },
                    new Border
                    {
                        BorderBrush = Brush.Red,
                        BorderThickness = new Thickness(10, 20, 30, 40),
                        Background = new GradientBrush(Color.Red, Color.Green, 90),
                        Margin = new Thickness(1),
                        Padding = new Thickness(10),
                        MinWidth = 300,
                        Content = new TextBlock("Irregular borders")
                    },
                    new Border
                    {
                        Margin = new Thickness(4),
                        BorderBrush = Brush.Green,
                        BorderThickness = new Thickness(1),
                        Padding = new Thickness(4),
                        Content = new TextBoxInput()
                    },
                    new Border
                    {
                        Margin = new Thickness(4),
                        BorderBrush = Brush.Green,
                        BorderThickness = new Thickness(1),
                        Padding = new Thickness(4),
                        Content = new ButtonInput
                        {
                            MaxWidth = 200
                        }
                    },
                    new Border
                    {
                        Margin = new Thickness(20),
                        Content = new StackPanel
                        {
                            Children =
                            {
                                BuildImages(StretchDirection.Both, NeutralResources.error),
                                BuildImages(StretchDirection.DownOnly, NeutralResources.error),
                                BuildImages(StretchDirection.UpOnly, NeutralResources.error),
                                new Border
                                {
                                    BorderBrush = Brush.Blue,
                                    BorderThickness = new Thickness(1),
                                    Content = new Image { Bitmap = NeutralResources.apple },
                                }
                            }
                        }
                    },
                    new Border
                    {
                        Margin = new Thickness(20),
                        Content = new StackPanel
                        {
                            Children =
                            {
                                BuildImages(StretchDirection.Both, NeutralResources.angel),
                                BuildImages(StretchDirection.DownOnly, NeutralResources.angel),
                                BuildImages(StretchDirection.UpOnly, NeutralResources.angel),
                                new Border
                                {
                                    BorderBrush = Brush.Blue,
                                    BorderThickness = new Thickness(1),
                                    Content = new Image { Bitmap = NeutralResources.apple },
                                }
                            }
                        }
                    },
                    new Border
                    {
                        BorderBrush = Brush.Green,
                        BorderThickness = new Thickness(1),
                        Background = Brush.Yellow,
                        Margin = new Thickness(2),
                        Padding = new Thickness(2),
                        MaxWidth = 300,
                        Content = new TextBlock
                        {
                            Wrap = true,
                            Runs =
                            {
                                new Run
                                {
                                    Text = "Hello "
                                },
                                new Run
                                {
                                    Text = "world! ",
                                    FontStyle = FontStyle.Bold
                                },
                                new Run
                                {
                                    Text = "Hello "
                                },
                                new Run
                                {
                                    Text = "world! ",
                                    FontStyle = FontStyle.Bold
                                },
                                new Run
                                {
                                    Text = "Hello "
                                },
                                new Run
                                {
                                    Text = "world! ",
                                    FontStyle = FontStyle.Bold
                                },
                                new Run
                                {
                                    Text = "Hello "
                                },
                                new Run
                                {
                                    Text = "world! ",
                                    FontStyle = FontStyle.Bold
                                },
                                new Run
                                {
                                    Text = "Hello "
                                },
                                new Run
                                {
                                    Text = "world! ",
                                    FontStyle = FontStyle.Bold
                                },
                                new Run
                                {
                                    Text = "Hello "
                                },
                                new Run
                                {
                                    Text = "world! ",
                                    FontStyle = FontStyle.Bold
                                },
                                new Run
                                {
                                    Text = "Hello "
                                },
                                new Run
                                {
                                    Text = "world! ",
                                    FontStyle = FontStyle.Bold
                                },
                                new Run
                                {
                                    Text = "Hello "
                                },
                                new Run
                                {
                                    Text = "world! ",
                                    FontStyle = FontStyle.Bold
                                },
                                new Run
                                {
                                    Text = "Hello "
                                },
                                new Run
                                {
                                    Text = "world! ",
                                    FontStyle = FontStyle.Bold
                                },
                                new Link
                                {
                                    Text = "http://google.com/"
                                }
                            }
                        }
                    }
                }
            };

            for (int i = 0; i < 20; i++)
            {
                var firstRun = new Run
                {
                    Text = "This is "
                };

                firstRun.MouseEnter += (s, ea) => ((Run)s).ForeColor = Color.Red;
                firstRun.MouseLeave += (s, ea) => ((Run)s).ForeColor = null;

                var secondRun = new Run
                {
                    Text = "line of text."
                };

                secondRun.MouseEnter += (s, ea) => ((Run)s).ForeColor = Color.Red;
                secondRun.MouseLeave += (s, ea) => ((Run)s).ForeColor = null;

                var childChild = new Border
                {
                    Padding = new Thickness(2),
                    Margin = new Thickness(2),
                    Background = Brush.Pink,
                    BorderBrush = Brush.Green,
                    BorderThickness = new Thickness(1),
                    Content = new TextBlock
                    {
                        Runs =
                        {
                            firstRun,
                            secondRun
                        }
                    }
                };

                childChild.MouseEnter += (s, e) =>
                {
                    ((Border)s).Background = Brush.Brown;
                    ((Border)s).Margin = new Thickness(10);
                };
                childChild.MouseLeave += (s, e) =>
                {
                    ((Border)s).Background = Brush.Gray;
                    ((Border)s).Margin = new Thickness(2);
                };

                var child = new Border
                {
                    Padding = new Thickness(10),
                    Margin = new Thickness(3),
                    Background = Brush.Lime,
                    BorderBrush = Brush.HotPink,
                    BorderThickness = new Thickness(1),
                    Content = childChild,
                    HorizontalAlignment = (HorizontalAlignment)(i % 4)
                };

                child.MouseEnter += (s, e) => ((Border)s).Background = Brush.Blue;
                child.MouseLeave += (s, e) => ((Border)s).Background = Brush.Lime;

                stackPanel.Children.Add(child);
            }

            return new Border
            {
                BorderBrush = Brush.Green,
                BorderThickness = new Thickness(1),
                Content = stackPanel,
                Background = Brush.White
            };
        }

        private Element BuildImages(StretchDirection stretchDirection, System.Drawing.Image bitmap)
        {
            return new StackPanel
            {
                Children =
                {
                    new Border
                    {
                        BorderBrush = Brush.Blue,
                        BorderThickness = new Thickness(1),
                        Content = new Image
                        {
                            MaxHeight = NeutralResources.apple.Height,
                            MaxWidth = NeutralResources.apple.Width,
                            Bitmap = bitmap,
                            Stretch = Stretch.Fill,
                            StretchDirection = stretchDirection
                        }
                    },
                    new Border
                    {
                        BorderBrush = Brush.Blue,
                        BorderThickness = new Thickness(1),
                        Content = new Image
                        {
                            MaxHeight = NeutralResources.apple.Height,
                            MaxWidth = NeutralResources.apple.Width,
                            Bitmap = bitmap,
                            Stretch = Stretch.None,
                            StretchDirection = stretchDirection
                        }
                    },
                    new Border
                    {
                        BorderBrush = Brush.Blue,
                        BorderThickness = new Thickness(1),
                        Content = new Image
                        {
                            MaxHeight = NeutralResources.apple.Height,
                            MaxWidth = 90,
                            Bitmap = bitmap,
                            Stretch = Stretch.Uniform,
                            MinWidth = 90,
                            StretchDirection = stretchDirection
                        }
                    },
                    new Border
                    {
                        BorderBrush = Brush.Blue,
                        BorderThickness = new Thickness(1),
                        Content = new Image
                        {
                            MaxHeight = NeutralResources.apple.Height,
                            MaxWidth = 90,
                            Bitmap = bitmap,
                            Stretch = Stretch.UniformToFill,
                            MinWidth = 90,
                            StretchDirection = stretchDirection
                        }
                    }
                }
            };
        }

        private class MyButton : Border
        {
            public MyButton()
            {
                Background = Brush.Blue;
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                Capture = true;

                Background = Brush.Yellow;
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                if (Capture)
                {
                    if (IsMouseDirectlyOver)
                        Background = Brush.Yellow;
                    else
                        Background = Brush.Green;
                }
            }

            protected override void OnMouseUp(MouseEventArgs e)
            {
                if (Capture)
                {
                    Capture = false;

                    Background = Brush.Blue;
                }
            }
        }
    }
}
