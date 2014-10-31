using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GdiPresentation.Win32;

namespace GdiPresentation
{
    public class ElementControl : CustomScrollControl
    {
        private readonly ElementHost _host;
        private ScrollBars _allowedScrollBars;
        private bool _processQueuePending;
        private Point _previousOffset;

        static ElementControl()
        {
            WheelMessageFilter.Install();
        }

        internal Element LastFocusedElement { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public Element Content
        {
            get { return _host.Content; }
            set { _host.Content = value; }
        }

        [DefaultValue(ScrollBars.None)]
        [Category("Layout")]
        public ScrollBars AllowedScrollBars
        {
            get { return _allowedScrollBars; }
            set
            {
                if (_allowedScrollBars != value)
                {
                    _allowedScrollBars = value;

                    PerformLayout();
                }
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        public ElementControl()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.Selectable, true);

            _host = new ElementHost(this);
        }

        protected override void OnDisplayRectangleChanged(EventArgs e)
        {
            var staticPanel = Content as StaticPanel;

            if (staticPanel != null)
            {
                // We get multiple events for the same location, so filter
                // them out.

                var currentOffset = (Point)DisplayRectangle.Location;

                if (_previousOffset != currentOffset)
                {
                    int xDelta = _previousOffset.X - currentOffset.X;
                    int yDelta = _previousOffset.Y - currentOffset.Y;

                    _previousOffset = currentOffset;

                    staticPanel.ApplyScroll(xDelta, yDelta);
                }
            }

            base.OnDisplayRectangleChanged(e);

            Update();
        }

        protected override void ScrollWindow(int xDelta, int yDelta, Rectangle rect)
        {
            var staticPanel = Content as StaticPanel;

            // Check for special cases. If any of these cases are true, the
            // base ScrollWindow knows how to handle this (i.e. nothing
            // special has to happen).

            if (
                staticPanel == null ||
                // If we don't have panels, we don't have to scroll specially.
                (staticPanel.LeftOffset == 0 && staticPanel.TopOffset == 0) ||
                // If we don't have a left panel, no special scroll is necessary
                // when scrolling horizontally.
                (staticPanel.LeftOffset == 0 && yDelta == 0) ||
                // If we don't have a top panel, no special scroll is necessary
                // when scrolling vertically.
                (staticPanel.TopOffset == 0 && xDelta == 0)
            ) {
                base.ScrollWindow(xDelta, yDelta, rect);
                return;
            }

            // Otherwise, we need to take the static panel into account.

            var client = ClientRectangle;

            if (xDelta != 0)
            {
                base.ScrollWindow(
                    xDelta,
                    0,
                    new Rectangle(
                        staticPanel.LeftOffset,
                        0,
                        client.Width - staticPanel.LeftOffset,
                        client.Height
                    )
                );
            }

            if (yDelta != 0)
            {
                base.ScrollWindow(
                    0,
                    yDelta,
                    new Rectangle(
                        0,
                        staticPanel.TopOffset,
                        client.Width,
                        client.Height - staticPanel.TopOffset
                    )
                );
            }
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            MeasureArrange(true);

            base.OnLayout(e);
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            MeasureArrange();

            Stopwatch stopwatch = null;
            bool forced = false;

            if (ElementStatistics.IsEnabled)
            {
                stopwatch = Stopwatch.StartNew();
                forced =
                    _host.LayoutManager != null &&
                    (_host.LayoutManager.PaintBoundsQueue.IsFull || _host.LayoutManager.PaintElementQueue.IsFull);
            }

            var scrollOffset = DisplayRectangle.Location;

            e.Graphics.TranslateTransform(
                scrollOffset.X,
                scrollOffset.Y
            );

            _host.RaisePaint(new PaintEventArgs(
                e.Graphics,
                new Rect(
                    e.ClipRectangle.Left - scrollOffset.X,
                    e.ClipRectangle.Top - scrollOffset.Y,
                    e.ClipRectangle.Width,
                    e.ClipRectangle.Height
                )
            ));

            e.Graphics.ResetTransform();

            if (stopwatch != null)
                ElementStatistics.AddEvent(ElementStatisticsEventType.Paint | (forced ? ElementStatisticsEventType.Forced : 0), stopwatch.Elapsed);
        }

        private void MeasureArrange()
        {
            MeasureArrange(false);
        }

        private void MeasureArrange(bool force)
        {
            if (_host.IsMeasureValid && _host.IsArrangeValid && !force)
                return;

            var clientSize = ClientSize;
            var minimalClientSize = (Size)clientSize;

            bool canHScroll = AllowedScrollBars == ScrollBars.Horizontal || AllowedScrollBars == ScrollBars.Both;
            bool canVScroll = AllowedScrollBars == ScrollBars.Vertical || AllowedScrollBars == ScrollBars.Both;

            if (HScroll)
                clientSize.Height += SystemInformation.HorizontalScrollBarHeight;
            else if (ShowHorizontalScrollBar != ScrollBarVisibility.Hidden)
                minimalClientSize.Height -= SystemInformation.HorizontalScrollBarHeight;

            if (VScroll)
                clientSize.Width += SystemInformation.VerticalScrollBarWidth;
            else if (ShowVerticalScrollBar != ScrollBarVisibility.Hidden)
                minimalClientSize.Width -= SystemInformation.VerticalScrollBarWidth;

            Stopwatch stopwatch = null;

            if (ElementStatistics.IsEnabled)
                stopwatch = Stopwatch.StartNew();

            var previousDesiredSize = _host.DesiredSize;

            _host.Measure(minimalClientSize);

            if (stopwatch != null)
                ElementStatistics.AddEvent(ElementStatisticsEventType.Measure | (force ? ElementStatisticsEventType.Forced : 0), stopwatch.Elapsed);

            var desiredSize = _host.DesiredSize;
            var finalSize = desiredSize;

            if (canHScroll || canVScroll)
            {
                bool willShowHScroll = CalculateNeedScroll(desiredSize.Width > clientSize.Width, ShowHorizontalScrollBar);
                bool willShowVScroll = CalculateNeedScroll(desiredSize.Height > clientSize.Height, ShowVerticalScrollBar);

                if (willShowHScroll && desiredSize.Height > minimalClientSize.Height)
                    willShowVScroll = ShowVerticalScrollBar == ScrollBarVisibility.Auto;
                if (willShowVScroll && desiredSize.Width > minimalClientSize.Width)
                    willShowHScroll = ShowHorizontalScrollBar == ScrollBarVisibility.Auto;

                if (willShowHScroll)
                    clientSize.Height -= SystemInformation.HorizontalScrollBarHeight;
                if (willShowVScroll)
                    clientSize.Width -= SystemInformation.VerticalScrollBarWidth;

                if (canHScroll)
                    finalSize.Width = Math.Max(finalSize.Width, clientSize.Width);
                if (canVScroll)
                    finalSize.Height = Math.Max(finalSize.Height, clientSize.Height);
            }

            if (!canHScroll)
                finalSize.Width = clientSize.Width;
            if (!canVScroll)
                finalSize.Height = clientSize.Height;

            if (ElementStatistics.IsEnabled)
                stopwatch = Stopwatch.StartNew();

            _host.Arrange(new Rect(new Point(0, 0), finalSize));

            if (stopwatch != null)
                ElementStatistics.AddEvent(ElementStatisticsEventType.Arrange | (force ? ElementStatisticsEventType.Forced : 0), stopwatch.Elapsed);

            var displaySize = _host.Size;

            if (!canHScroll)
                displaySize.Width = Math.Min(displaySize.Width, clientSize.Width);
            if (!canVScroll)
                displaySize.Height = Math.Min(displaySize.Height, clientSize.Height);

            SetDisplaySize((System.Drawing.Size)displaySize);

            // Under certain circumstances, when the size of the content shrunk,
            // we need to specifically invalidate the area that became visible
            // because that area is not part of an already invalidated area.
            // We only do this when the area is less than the final size because
            // this applies when we don't have scrollbars.

            if (
                desiredSize.Height < finalSize.Height &&
                desiredSize.Height < previousDesiredSize.Height
            ) {
                Invalidate(new Rectangle(
                    0,
                    desiredSize.Height,
                    finalSize.Width,
                    previousDesiredSize.Height - desiredSize.Height
                ));
            }

            if (
                desiredSize.Width < finalSize.Width &&
                desiredSize.Width < previousDesiredSize.Width
            ) {
                Invalidate(new Rectangle(
                    desiredSize.Width,
                    0,
                    previousDesiredSize.Width - desiredSize.Width,
                    finalSize.Height
                ));
            }
        }

        private static bool CalculateNeedScroll(bool visible, ScrollBarVisibility visibility)
        {
            switch (visibility)
            {
                case ScrollBarVisibility.Hidden: return false;
                case ScrollBarVisibility.Visible: return true;
                default: return visible;
            }
        }

        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseMove(e);

            _host.RaiseMouseMove(BuildElementMouseEventArgs(e));
        }

        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseUp(e);

            _host.RaiseMouseUp(BuildElementMouseEventArgs(e));
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (!ContainsFocus)
                Focus();

            _host.RaiseMouseDown(BuildElementMouseEventArgs(e));
        }

        protected override void OnMouseDoubleClick(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (!ContainsFocus)
                Focus();

            _host.RaiseMouseDoubleClick(BuildElementMouseEventArgs(e));
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            _host.PrepareMouseLeave();
            _host.ProcessMouseLeave();
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);

            _host.RaiseMouseMove(BuildElementMouseEventArgs());
        }

        private MouseEventArgs BuildElementMouseEventArgs()
        {
            return new MouseEventArgs(
                (Point)MousePosition,
                (MouseButtons)MouseButtons,
                0,
                0
            );
        }

        private MouseEventArgs BuildElementMouseEventArgs(System.Windows.Forms.MouseEventArgs e)
        {
            return new MouseEventArgs(
                (Point)PointToScreen(e.Location),
                (MouseButtons)e.Button,
                e.Clicks,
                e.Delta
            );
        }

        internal void QueueInvalidate()
        {
            if (!_processQueuePending)
            {
                _processQueuePending = true;

                BeginInvoke(new Action(ProcessQueue));
            }
        }

        private void ProcessQueue()
        {
            MeasureArrange();

            var elementQueue = _host.LayoutManager.PaintElementQueue;
            var boundsQueue = _host.LayoutManager.PaintBoundsQueue;

            if (elementQueue.IsEmpty && boundsQueue.IsEmpty)
                return;

            if (elementQueue.IsFull || boundsQueue.IsFull)
            {
                Invalidate();
            }
            else
            {
                // All elements in the elementQueue have now been arranged.
                // Push them to the boundsQueue.

                for (int i = 0, count = elementQueue.Count; i < count; i++)
                {
                    var element = elementQueue.GetItem(i);

                    element.Invalidate(new Rect(Point.Empty, element.Size));
                }

                if (boundsQueue.IsFull)
                {
                    Invalidate();
                }
                else
                {
                    using (var region = new Region(Rectangle.Empty))
                    {
                        var scrollOffset = DisplayRectangle.Location;

                        for (int i = 0, count = boundsQueue.Count; i < count; i++)
                        {
                            var bounds = boundsQueue.GetItem(i);

                            region.Union(new Rectangle(
                                bounds.Left + scrollOffset.X,
                                bounds.Top + scrollOffset.Y,
                                bounds.Width,
                                bounds.Height
                            ));
                        }

                        Invalidate(region);
                    }
                }
            }

            elementQueue.Reset();
            boundsQueue.Reset();

            _processQueuePending = false;

            Update();
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                case NativeMethods.WM_ERASEBKGND:
                    return;

                case NativeMethods.WM_SETCURSOR:
                    WmSetCursor(ref m);
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        private void WmSetCursor(ref System.Windows.Forms.Message m)
        {
            // Only resolve the cursor when we're in the client bounds. The
            // scrollbars must also get a chance to resolve the cursor.

            var point = _host.Control.PointToClient(System.Windows.Forms.Cursor.Position);

            if (ClientSize.Width < point.X || ClientSize.Height < point.Y)
            {
                DefWndProc(ref m);
                return;
            }

            var e = new ResolveCursorEventArgs(
                (MouseButtons)MouseButtons,
                0,
                0
            );

            _host.RaiseResolveCursor(e);

            System.Windows.Forms.Cursor.Current =
                e.Cursor != null
                ? e.Cursor.NativeCursor
                : Cursor;
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if ((keyData & Keys.KeyCode) == Keys.Tab && (keyData & ~(Keys.Tab | Keys.Shift)) == 0)
            {
                bool forward = (keyData & Keys.Shift) == 0;

                if (_host.FocusedElement != null)
                {
                    var next = _host.FocusedElement.FindNextElement(true, false, forward);

                    if (next != null)
                    {
                        next.Focus();

                        return true;
                    }
                }
            }

            return base.ProcessDialogKey(keyData);
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);

            LastFocusedElement = null;
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);

            var lastFocused = _host.FocusedElement;

            _host.FocusedElement = null;

            LastFocusedElement = lastFocused;
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            if (LastFocusedElement != null)
                LastFocusedElement.Focus();
        }

        protected override void Select(bool directed, bool forward)
        {
            if (directed && Content != null)
            {
                var toSelect =
                    forward
                    ? Content.FindSelfOrNextElement(true /* focusable */, false /* wrap */, true /* forward */)
                    : Content.FindNextElement(true /* focusable */, false /* wrap */, false /* forward */);

                if (toSelect != null)
                    toSelect.Focus();
            }

            base.Select(directed, forward);
        }

        private class WheelMessageFilter : IMessageFilter
        {
            static WheelMessageFilter()
            {
                Application.AddMessageFilter(new WheelMessageFilter());
            }

            public static void Install()
            {
            }

            public bool PreFilterMessage(ref System.Windows.Forms.Message m)
            {
                if (m.Msg == NativeMethods.WM_MOUSEWHEEL)
                {
                    var pos = new System.Drawing.Point(
                        NativeMethods.Util.LOWORD(m.LParam),
                        NativeMethods.Util.HIWORD(m.LParam)
                    );

                    var handle = NativeMethods.WindowFromPoint(pos);

                    if (handle != IntPtr.Zero)
                    {
                        var control = Control.FromHandle(handle);
                        
                        while (control != null && !(control is ElementControl))
                        {
                            control = control.Parent;
                        }

                        if (control != null)
                        {
                            NativeMethods.SendMessage(control.Handle, m.Msg, m.WParam, m.LParam);
                            return true;
                        }
                    }
                }

                return false;
            }
        }
    }
}
