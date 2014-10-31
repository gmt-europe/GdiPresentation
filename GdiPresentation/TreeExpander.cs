using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

namespace GdiPresentation
{
    public class TreeExpander : HeaderedContainerElement
    {
        public const int DefaultExpanderWidth = 19;

        private int _expanderWidth = DefaultExpanderWidth;
        private bool _isExpanded;
        private Thickness _containerMargin;
        private ContainerElement _parent;
        private bool _paintPending = true;
        private bool _haveNext;
        private bool _havePrevious;
        private TreeExpander _parentTreeExpander;

        public int ExpanderWidth
        {
            get { return _expanderWidth; }
            set
            {
                if (_expanderWidth != value)
                {
                    _expanderWidth = value;
                    InvalidateMeasure();
                }
            }
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;

                    for (int i = 0, count = Children.Count; i < count; i++)
                    {
                        Children[i].Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                    }

                    InvalidateMeasure();

                    OnIsExpandedChanged(EventArgs.Empty);
                }
            }
        }

        private bool IsExpandedAndHasChildren
        {
            get { return IsExpanded && Children.Count > 0; }
        }

        public event EventHandler IsExpandedChanged;

        protected virtual void OnIsExpandedChanged(EventArgs e)
        {
            var ev = IsExpandedChanged;
            if (ev != null)
                ev(this, e);
        }

        public Thickness ContainerMargin
        {
            get { return _containerMargin; }
            set
            {
                if (_containerMargin != value)
                {
                    _containerMargin = value;
                    InvalidateMeasure();
                }
            }
        }

        public TreeExpander()
        {
            Background = Brush.Transparent;
        }

        protected override Size MeasureOverride(Size desiredSize)
        {
            int sum = 0;
            int max = 0;

            var size = new Size(
                desiredSize.Width - (ExpanderWidth + ContainerMargin.Horizontal),
                int.MaxValue
            );

            if (Header != null)
                MeasureChild(Header, size, ref sum, ref max);

            if (IsExpanded)
            {
                for (int i = 0, count = Children.Count; i < count; i++)
                {
                    MeasureChild(Children[i], size, ref sum, ref max);
                }
            }

            return new Size(
                max + ExpanderWidth + ContainerMargin.Horizontal,
                sum + (IsExpandedAndHasChildren ? ContainerMargin.Vertical : 0)
            );
        }

        private static void MeasureChild(Element item, Size size, ref int sum, ref int max)
        {
            item.Measure(size);

            sum += item.DesiredSize.Height;
            max = Math.Max(max, item.DesiredSize.Width);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (!IsExpanded)
            {
                for (int i = 0, count = Children.Count; i < count; i++)
                {
                    Children[i].Visibility = Visibility.Collapsed;
                }
            }

            int sum = 0;

            if (Header == null && IsExpandedAndHasChildren)
                sum = ContainerMargin.Top;

            if (Header != null)
                ArrangeChild(finalSize, Header, ref sum);

            if (IsExpanded)
            {
                for (int i = 0, count = Children.Count; i < count; i++)
                {
                    ArrangeChild(finalSize, Children[i], ref sum);
                }
            }

            if (IsExpandedAndHasChildren)
                sum += ContainerMargin.Bottom;

            return new Size(
                finalSize.Width + ExpanderWidth + ContainerMargin.Horizontal,
                Math.Max(finalSize.Height, sum)
            );
        }

        private void ArrangeChild(Size finalSize, Element item, ref int sum)
        {
            item.Visibility = Visibility.Visible;

            var bounds = new Rect(
                ExpanderWidth,
                sum,
                finalSize.Width - ExpanderWidth,
                Math.Min(item.DesiredSize.Height, finalSize.Height - sum)
                );

            if (item != Header)
            {
                bounds = new Rect(
                    bounds.Left + ContainerMargin.Left,
                    bounds.Top,
                    bounds.Width - ContainerMargin.Horizontal,
                    bounds.Height
                    );
            }
            else if (IsExpandedAndHasChildren)
            {
                sum += ContainerMargin.Top;
            }

            item.Arrange(bounds);

            sum += item.ActualHeight + item.Margin.Vertical;
        }

        protected override void OnPaint(ElementPaintEventArgs e)
        {
            // Without a header it gets very difficult to draw correct lines.
            // Also, we don't do anything if we don't have enough room to draw
            // the boxes. This allows the caller to hide the expander by setting
            // the width to 0.

            _paintPending = false;

            if (Header == null || ExpanderWidth < 9)
                return;

            var toggleOffset = GetToggleOffset(e.Bounds.Location);

            if (_parent == null)
                return;

            var index = CalculateHaveSiblings();

            using (var pen = new Pen(SystemColors.ControlDark, 1)
            {
                DashStyle = DashStyle.Dot
            })
            {
                int left = toggleOffset.X + 4;

                if (_havePrevious)
                {
                    int top = e.Bounds.Top - Margin.Top;

                    if (index == 0 && _parentTreeExpander != null)
                        top -= _parentTreeExpander.ContainerMargin.Top;

                    e.Graphics.DrawLine(
                        pen,
                        left,
                        RoundTop(top),
                        left,
                        toggleOffset.Y + 4
                    );
                }

                if (_haveNext)
                {
                    e.Graphics.DrawLine(
                        pen,
                        left,
                        RoundTop(toggleOffset.Y + 4),
                        left,
                        e.Bounds.Bottom + Margin.Bottom
                    );
                }

                e.Graphics.DrawLine(
                    pen,
                    left + (toggleOffset.Y % 2 == 0 ? 0 : 1),
                    toggleOffset.Y + 4,
                    e.Bounds.Left + ExpanderWidth,
                    toggleOffset.Y + 4
                );
            }

            if (Children.Count <= 0)
                return;

            e.Graphics.DrawRectangle(
                SystemPens.ControlDark,
                toggleOffset.X,
                toggleOffset.Y,
                8,
                8
            );

            e.Graphics.FillRectangle(
                SystemBrushes.Window,
                toggleOffset.X + 1,
                toggleOffset.Y + 1,
                7,
                7
            );

            e.Graphics.DrawLine(
                SystemPens.ControlText,
                toggleOffset.X + 2,
                toggleOffset.Y + 4,
                toggleOffset.X + 6,
                toggleOffset.Y + 4
            );

            if (Children.Count > 0 && !IsExpanded)
            {
                e.Graphics.DrawLine(
                    SystemPens.ControlText,
                    toggleOffset.X + 4,
                    toggleOffset.Y + 2,
                    toggleOffset.X + 4,
                    toggleOffset.Y + 6
                );
            }
        }

        private int CalculateHaveSiblings()
        {
            int index = _parent.Children.IndexOf(this);

            _haveNext = GetVisibleSibling(index, 1) is TreeExpander;

            _havePrevious =
                _parentTreeExpander != null ||
                GetVisibleSibling(index, -1) is TreeExpander;

            return index;
        }

        private Element GetVisibleSibling(int index, int offset)
        {
            var children = _parent.Children;
            int count = children.Count;

            for (index += offset; index >= 0 && index < count; index += offset)
            {
                var element = children[index];

                if (element.Visibility == Visibility.Visible)
                    return element;
            }

            return null;
        }

        private static int RoundTop(int y)
        {
            return ((y + 1) / 2) * 2;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            var location = e.GetPosition(this);

            var toggleOffset = GetToggleOffset(Point.Empty);
            var toggleBounds = new Rect(
                toggleOffset.X,
                toggleOffset.Y,
                9,
                9
            );

            if (toggleBounds.Contains(location))
                IsExpanded = !IsExpanded;
        }

        private Point GetToggleOffset(Point offset)
        {
            if (Header == null)
                return Point.Empty;

            return new Point(
                offset.X + (ExpanderWidth - 9) / 2,
                offset.Y + (Header.ActualHeight - 9) / 2
            );
        }

        protected override void OnLoaded(EventArgs e)
        {
            base.OnLoaded(e);

            _parent = Parent as ContainerElement;
            _parentTreeExpander = _parent as TreeExpander;

            if (_parent != null)
                _parent.Children.CollectionChanged += Children_CollectionChanged;
        }

        void Children_CollectionChanged(object sender, EventArgs e)
        {
            if (_paintPending)
                return;

            bool hadNext = _haveNext;
            bool hadPrevious = _havePrevious;

            CalculateHaveSiblings();

            if (hadNext != _haveNext || hadPrevious != _havePrevious)
            {
                _paintPending = true;
                Invalidate();
            }
        }

        protected override void OnUnloaded(EventArgs e)
        {
            base.OnUnloaded(e);

            if (_parent != null)
            {
                _parent.Children.CollectionChanged -= Children_CollectionChanged;
                _parent = null;
            }
        }
    }
}
