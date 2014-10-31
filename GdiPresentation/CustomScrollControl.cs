using System.Runtime.InteropServices;
using System;
using System.Drawing;
using System.Security.Permissions;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using GdiPresentation.Win32;

namespace GdiPresentation
{
    public class CustomScrollControl : Control
    {
        private System.Drawing.Size _displaySize;
        private Rectangle _displayRect;
        private readonly ScrollProperties _verticalScroll;
        private readonly ScrollProperties _horizontalScroll;
        private bool _allowResize;
        private VisualStyleRenderer _sizeGripRenderer;
        private ScrollBarVisibility _showHorizontalScrollBar;
        private ScrollBarVisibility _showVerticalScrollBar;
        private System.Drawing.Point? _dragStart;
        private System.Drawing.Size _startSize;

        public event ScrollEventHandler Scroll;

        protected virtual void OnScroll(ScrollEventArgs se)
        {
            var ev = Scroll;

            if (ev != null)
                ev(this, se);
        }

        public event EventHandler DisplayRectangleChanged;

        protected virtual void OnDisplayRectangleChanged(EventArgs e)
        {
            var ev = DisplayRectangleChanged;
            if (ev != null)
                ev(this, e);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;

                if (HScroll || _horizontalScroll.Visible)
                    cp.Style |= NativeMethods.WS_HSCROLL;
                else
                    cp.Style &= (~NativeMethods.WS_HSCROLL);
                if (VScroll || _verticalScroll.Visible)
                    cp.Style |= NativeMethods.WS_VSCROLL;
                else
                    cp.Style &= (~NativeMethods.WS_VSCROLL);

                return cp;
            }
        }

        public override Rectangle DisplayRectangle
        {
            get
            {
                Rectangle rect = ClientRectangle;

                if (!_displayRect.IsEmpty)
                {
                    rect.X = _displayRect.X;
                    rect.Y = _displayRect.Y;

                    if (HScroll)
                        rect.Width = _displayRect.Width;
                    if (VScroll)
                        rect.Height = _displayRect.Height;
                }

                return rect;
            }
        }

        [Browsable(false)]
        public bool HScroll { get; private set; }

        [Browsable(false)]
        public bool VScroll { get; private set; }

        [DefaultValue(false)]
        public bool AllowResize
        {
            get { return _allowResize; }
            set
            {
                if (_allowResize != value)
                {
                    _allowResize = value;

                    if (IsHandleCreated)
                        NativeMethods.SendMessage(Handle, NativeMethods.WM_NCPAINT, (IntPtr)1, (IntPtr)0);
                }
            }
        }

        [Browsable(false)]
        public Control ResizeTarget { get; set; }

        [DefaultValue(ScrollBarVisibility.Auto)]
        public ScrollBarVisibility ShowHorizontalScrollBar
        {
            get { return _showHorizontalScrollBar; }
            set
            {
                if (_showHorizontalScrollBar != value)
                {
                    _showHorizontalScrollBar = value;

                    AdjustFormScrollbars();
                }
            }
        }

        [DefaultValue(ScrollBarVisibility.Auto)]
        public ScrollBarVisibility ShowVerticalScrollBar
        {
            get { return _showVerticalScrollBar; }
            set
            {
                if (_showVerticalScrollBar != value)
                {
                    _showVerticalScrollBar = value;

                    AdjustFormScrollbars();
                }
            }
        }

        public CustomScrollControl()
        {
            SetStyle(ControlStyles.ContainerControl, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, false);

            _horizontalScroll = new ScrollProperties(this, NativeMethods.SB_HORZ);
            _verticalScroll = new ScrollProperties(this, NativeMethods.SB_VERT);
        }

        protected void SetDisplaySize(System.Drawing.Size size)
        {
            if (_displaySize != size)
            {
                _displaySize = size;

                AdjustFormScrollbars();
            }
        }

        private void AdjustFormScrollbars()
        {
            if (ApplyScrollbarChanges())
                PerformLayout();
        }

        private bool ApplyScrollbarChanges()
        {
            var currentClient = ClientRectangle;
            var fullClient = currentClient;
            var minClient = fullClient;

            if (HScroll)
                fullClient.Height += SystemInformation.HorizontalScrollBarHeight;
            else
                minClient.Height -= SystemInformation.HorizontalScrollBarHeight;

            if (VScroll)
                fullClient.Width += SystemInformation.VerticalScrollBarWidth;
            else
                minClient.Width -= SystemInformation.VerticalScrollBarWidth;

            int maxX = _displaySize.Width;
            int maxY = _displaySize.Height;

            // Check maxX/maxY against the clientRect, we must compare it to the
            // clientRect without any scrollbars, and then we can check it against
            // the clientRect with the "new" scrollbars. This will make the 
            // scrollbars show and hide themselves correctly at the boundaries.
            // 
            bool needHscroll = CalculateNeedScroll(maxX > fullClient.Width, _showHorizontalScrollBar);
            bool needVscroll = CalculateNeedScroll(maxY > fullClient.Height, _showVerticalScrollBar);

            var clientToBe = fullClient;

            if (needHscroll)
                clientToBe.Height -= SystemInformation.HorizontalScrollBarHeight;
            if (needVscroll)
                clientToBe.Width -= SystemInformation.VerticalScrollBarWidth;

            if (needHscroll && maxY > clientToBe.Height)
                needVscroll = _showVerticalScrollBar != ScrollBarVisibility.Hidden;
            if (needVscroll && maxX > clientToBe.Width)
                needHscroll = _showHorizontalScrollBar != ScrollBarVisibility.Hidden;

            bool needLayout = SetVisibleScrollbars(needHscroll, needVscroll);

            if (HScroll || VScroll)
                needLayout = (SetDisplayRectangleSize(maxX, maxY) || needLayout);
            else
                SetDisplayRectangleSize(maxX, maxY);

            SyncScrollbars();

            return needLayout;
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

        protected override void OnLayout(LayoutEventArgs e)
        {
            AdjustFormScrollbars();

            base.OnLayout(e);
        }

        protected override void OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
        {
            // Favor the vertical scroll bar, since it's the most
            // common use.  However, if there isn't a vertical
            // scroll and the horizontal is on, then wheel it around.

            if (VScroll)
            {
                var client = ClientRectangle;
                int pos = -_displayRect.Y;
                int maxPos = -(client.Height - _displayRect.Height);

                pos = Math.Max(pos - e.Delta, 0);
                pos = Math.Min(pos, maxPos);

                SetDisplayRectLocation(_displayRect.X, -pos);
                SyncScrollbars();

                if (e is HandledMouseEventArgs)
                    ((HandledMouseEventArgs)e).Handled = true;
            }
            else if (HScroll)
            {
                var client = ClientRectangle;
                int pos = -_displayRect.X;
                int maxPos = -(client.Width - _displayRect.Width);

                pos = Math.Max(pos - e.Delta, 0);
                pos = Math.Min(pos, maxPos);

                SetDisplayRectLocation(-pos, _displayRect.Y);
                SyncScrollbars();

                if (e is HandledMouseEventArgs)
                    ((HandledMouseEventArgs)e).Handled = true;
            }

            base.OnMouseWheel(e);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            if (Visible)
                PerformLayout();

            base.OnVisibleChanged(e);
        }

        public void SetDisplayRectangleLocation(System.Drawing.Point location)
        {
            SetDisplayRectLocation(location.X, location.Y);

            SyncScrollbars();
        }

        private void SetDisplayRectLocation(int x, int y)
        {
            int xDelta = 0;
            int yDelta = 0;

            var client = ClientRectangle;
            var displayRectangle = _displayRect;
            int minX = Math.Min(client.Width - displayRectangle.Width, 0);
            int minY = Math.Min(client.Height - displayRectangle.Height, 0);

            if (x > 0)
                x = 0;
            if (x < minX)
                x = minX;
            if (y > 0)
                y = 0;
            if (y < minY)
                y = minY;

            if (displayRectangle.X != x)
                xDelta = x - displayRectangle.X;
            if (displayRectangle.Y != y)
                yDelta = y - displayRectangle.Y;

            _displayRect.X = x;
            _displayRect.Y = y;

            if (xDelta != 0 || yDelta != 0)
            {
                if (IsHandleCreated)
                    ScrollWindow(xDelta, yDelta, ClientRectangle);

                OnDisplayRectangleChanged(EventArgs.Empty);
            }
        }

        protected virtual void ScrollWindow(int xDelta, int yDelta, Rectangle rect)
        {
            var rcClip = new NativeMethods.RECT(rect);

            NativeMethods.ScrollWindowEx(
                new HandleRef(this, Handle),
                xDelta,
                yDelta,
                IntPtr.Zero,
                ref rcClip,
                IntPtr.Zero,
                IntPtr.Zero,
                NativeMethods.SW_INVALIDATE | NativeMethods.SW_SCROLLCHILDREN
            );
        }

        private int ScrollThumbPosition(int fnBar)
        {
            var si = new NativeMethods.SCROLLINFO
            {
                fMask = NativeMethods.SIF_TRACKPOS
            };

            NativeMethods.GetScrollInfo(new HandleRef(this, Handle), fnBar, si);

            return si.nTrackPos;
        }

        private void ResetScrollProperties(ScrollProperties scrollProperties)
        {
            // Set only these two values as when the ScrollBars are not visible ...
            // there is no meaning of the "value" property.

            scrollProperties.Visible = false;
            scrollProperties.Value = 0;
        }

        private bool SetVisibleScrollbars(bool horiz, bool vert)
        {
            // For some reason, the window style can become out of sync with
            // HScroll and VScroll. Because of this, we read the window style
            // here instead of HScroll/VScroll to ensure everything keeps
            // in sync.

            var styles = NativeMethods.GetWindowLong(Handle, NativeMethods.GWL_STYLE);

            bool styleHScroll = ((styles & NativeMethods.WS_HSCROLL) != 0);
            bool styleVScroll = ((styles & NativeMethods.WS_VSCROLL) != 0);

            bool needLayout = horiz != styleHScroll || vert != styleVScroll;

            if (needLayout)
            {
                int x = _displayRect.X;
                int y = _displayRect.Y;

                if (!horiz)
                    x = 0;
                if (!vert)
                    y = 0;

                SetDisplayRectLocation(x, y);
                HScroll = horiz;
                VScroll = vert;

                //Update the visible member of ScrollBars....
                if (horiz)
                    _horizontalScroll.Visible = true;
                else
                    ResetScrollProperties(_horizontalScroll);
                if (vert)
                    _verticalScroll.Visible = true;
                else
                    ResetScrollProperties(_verticalScroll);

                UpdateStyles();
            }

            return needLayout;
        }

        private bool SetDisplayRectangleSize(int width, int height)
        {
            bool needLayout = false;

            if (_displayRect.Width != width || _displayRect.Height != height)
            {
                _displayRect.Width = width;
                _displayRect.Height = height;
                needLayout = true;
            }

            int minX = ClientRectangle.Width - width;
            int minY = ClientRectangle.Height - height;

            if (minX > 0)
                minX = 0;
            if (minY > 0)
                minY = 0;

            int x = _displayRect.X;
            int y = _displayRect.Y;

            if (!HScroll)
                x = 0;
            if (!VScroll)
                y = 0;

            if (x < minX)
                x = minX;
            if (y < minY)
                y = minY;

            SetDisplayRectLocation(x, y);

            return needLayout;
        }

        private void SyncScrollbars()
        {
            if (!IsHandleCreated)
                return;

            Rectangle displayRect = _displayRect;

            if (HScroll)
            {
                _horizontalScroll.Maximum = displayRect.Width - 1;
                _horizontalScroll.LargeChange = ClientRectangle.Width;
                _horizontalScroll.SmallChange = 5;

                if (-displayRect.X >= 0 && -displayRect.X < _horizontalScroll.Maximum)
                    _horizontalScroll.Value = -displayRect.X;

                _horizontalScroll.UpdateScrollInfo();
            }

            if (VScroll)
            {
                _verticalScroll.Maximum = displayRect.Height - 1;
                _verticalScroll.LargeChange = ClientRectangle.Height;
                _verticalScroll.SmallChange = 5;

                if (-displayRect.Y >= 0 && -displayRect.Y < _verticalScroll.Maximum)
                    _verticalScroll.Value = -displayRect.Y;

                _verticalScroll.UpdateScrollInfo();
            }
        }

        private void WmVScroll(ref System.Windows.Forms.Message m)
        {
            if (m.LParam != IntPtr.Zero)
            {
                base.WndProc(ref m);
                return;
            }

            int pos = -_displayRect.Y;
            int oldValue = pos;

            int maxPos = _verticalScroll.Maximum;

            switch (NativeMethods.Util.LOWORD(m.WParam))
            {
                case NativeMethods.SB_THUMBPOSITION:
                case NativeMethods.SB_THUMBTRACK:
                    pos = ScrollThumbPosition(NativeMethods.SB_VERT);
                    break;

                case NativeMethods.SB_LINEUP:
                    if (pos > 0)
                        pos -= _verticalScroll.SmallChange;
                    else
                        pos = 0;
                    break;

                case NativeMethods.SB_LINEDOWN:
                    if (pos < maxPos - _verticalScroll.SmallChange)
                        pos += _verticalScroll.SmallChange;
                    else
                        pos = maxPos;
                    break;

                case NativeMethods.SB_PAGEUP:
                    if (pos > _verticalScroll.LargeChange)
                        pos -= _verticalScroll.LargeChange;
                    else
                        pos = 0;
                    break;

                case NativeMethods.SB_PAGEDOWN:
                    if (pos < maxPos - _verticalScroll.LargeChange)
                        pos += _verticalScroll.LargeChange;
                    else
                        pos = maxPos;
                    break;

                case NativeMethods.SB_TOP:
                    pos = 0;
                    break;

                case NativeMethods.SB_BOTTOM:
                    pos = maxPos;
                    break;
            }

            SetDisplayRectLocation(_displayRect.X, -pos);
            SyncScrollbars();

            WmOnScroll(ref m, oldValue, pos, ScrollOrientation.VerticalScroll);
        }

        private void WmHScroll(ref System.Windows.Forms.Message m)
        {
            if (m.LParam != IntPtr.Zero)
            {
                base.WndProc(ref m);
                return;
            }

            int pos = -_displayRect.X;
            int oldValue = pos;
            int maxPos = _horizontalScroll.Maximum;

            switch (NativeMethods.Util.LOWORD(m.WParam))
            {
                case NativeMethods.SB_THUMBPOSITION:
                case NativeMethods.SB_THUMBTRACK:
                    pos = ScrollThumbPosition(NativeMethods.SB_HORZ);
                    break;

                case NativeMethods.SB_LINEUP:
                    if (pos > _horizontalScroll.SmallChange)
                        pos -= _horizontalScroll.SmallChange;
                    else
                        pos = 0;
                    break;

                case NativeMethods.SB_LINEDOWN:
                    if (pos < maxPos - _horizontalScroll.SmallChange)
                        pos += _horizontalScroll.SmallChange;
                    else
                        pos = maxPos;
                    break;

                case NativeMethods.SB_PAGEUP:
                    if (pos > _horizontalScroll.LargeChange)
                        pos -= _horizontalScroll.LargeChange;
                    else
                        pos = 0;
                    break;

                case NativeMethods.SB_PAGEDOWN:
                    if (pos < maxPos - _horizontalScroll.LargeChange)
                        pos += _horizontalScroll.LargeChange;
                    else
                        pos = maxPos;
                    break;

                case NativeMethods.SB_LEFT:
                    pos = 0;
                    break;

                case NativeMethods.SB_RIGHT:
                    pos = maxPos;
                    break;
            }

            SetDisplayRectLocation(-pos, _displayRect.Y);
            SyncScrollbars();

            WmOnScroll(ref m, oldValue, pos, ScrollOrientation.HorizontalScroll);
        }

        private void WmOnScroll(ref System.Windows.Forms.Message m, int oldValue, int value, ScrollOrientation scrollOrientation)
        {
            var type = (ScrollEventType)NativeMethods.Util.LOWORD(m.WParam);

            if (type != ScrollEventType.EndScroll)
                OnScroll(new ScrollEventArgs(type, oldValue, value, scrollOrientation));
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                case NativeMethods.WM_VSCROLL:
                    WmVScroll(ref m);
                    break;

                case NativeMethods.WM_HSCROLL:
                    WmHScroll(ref m);
                    break;

                case NativeMethods.WM_NCPAINT:
                    WmNCPaint(ref m);
                    break;

                case NativeMethods.WM_NCHITTEST:
                    WmNCHitTest(ref m);
                    break;

                case NativeMethods.WM_NCLBUTTONDOWN:
                    WmNCLButtonDown(ref m);
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        private void WmNCLButtonDown(ref Message m)
        {
            var location = PointToClient(NativeMethods.Util.GetPoint(m.LParam));

            if (
                !(_allowResize && HScroll && VScroll) ||
                ResizeTarget == null ||
                !GetResizeGripBounds().Contains(location)
            ) {
                base.WndProc(ref m);
                return;
            }

            _startSize = ResizeTarget.Size;
            Capture = true;
            _dragStart = PointToScreen(location);
        }

        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if (Capture && _dragStart.HasValue)
            {
                var current = PointToScreen(e.Location);

                var delta = new System.Drawing.Size(
                    current.X - _dragStart.Value.X,
                    current.Y - _dragStart.Value.Y
                );

                ResizeTarget.Size = _startSize + delta;
            }
            else
            {
                base.OnMouseMove(e);
            }
        }

        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            if (Capture && _dragStart.HasValue)
            {
                Capture = false;
                _dragStart = null;
            }
            else
            {
                base.OnMouseUp(e);
            }
        }

        private void WmNCHitTest(ref System.Windows.Forms.Message m)
        {
            if (_allowResize && HScroll && VScroll)
            {
                // Convert to client coordinates
                //
                var pt = PointToClient(NativeMethods.Util.GetPoint(m.LParam));

                if (GetResizeGripBounds().Contains(pt))
                {
                    m.Result = (IntPtr)NativeMethods.HTBOTTOMRIGHT;
                    return;
                }
            }

            base.WndProc(ref m);
        }

        private void WmNCPaint(ref System.Windows.Forms.Message m)
        {
            base.WndProc(ref m);

            if (_allowResize && HScroll && VScroll)
            {
                var hdc = NativeMethods.GetDCEx(Handle, m.WParam, NativeMethods.DeviceContextValues.Window | NativeMethods.DeviceContextValues.IntersectRgn);

                if (hdc == IntPtr.Zero)
                    hdc = NativeMethods.GetWindowDC(Handle);

                try
                {
                    using (var graphics = Graphics.FromHdc(hdc))
                    {
                        var bounds = GetResizeGripBounds();

                        if (Application.RenderWithVisualStyles)
                        {
                            if (_sizeGripRenderer == null)
                                _sizeGripRenderer = new VisualStyleRenderer(VisualStyleElement.Status.Gripper.Normal);

                            graphics.FillRectangle(SystemBrushes.Control, bounds);

                            _sizeGripRenderer.DrawBackground(graphics, bounds);
                        }
                        else
                        {
                            ControlPaint.DrawSizeGrip(graphics, SystemColors.Control, bounds);
                        }
                    }
                }
                finally
                {
                    NativeMethods.ReleaseDC(Handle, hdc);
                }
            }
        }

        private Rectangle GetResizeGripBounds()
        {
            if (!_allowResize || !HScroll || !VScroll)
                return Rectangle.Empty;

            return new Rectangle(
                Width - SystemInformation.VerticalScrollBarWidth,
                Height - SystemInformation.HorizontalScrollBarHeight,
                SystemInformation.VerticalScrollBarWidth,
                SystemInformation.HorizontalScrollBarHeight
            );
        }

        private class ScrollProperties
        {
            private int _smallChange = 1;
            private int _largeChange = 10;
            private readonly int _orientation;
            private readonly CustomScrollControl _parentControl;

            public ScrollProperties(CustomScrollControl container, int orientation)
            {
                _parentControl = container;
                _orientation = orientation;

                Maximum = 100;
            }

            public int Maximum { get; set; }

            public int Value { get; set; }

            public bool Visible { get; set; }

            public int LargeChange
            {
                get { return Math.Min(_largeChange, Maximum + 1); }
                set { _largeChange = value; }
            }

            public int SmallChange
            {
                get { return Math.Min(_smallChange, LargeChange); }
                set { _smallChange = value; }
            }

            public void UpdateScrollInfo()
            {
                if (!_parentControl.IsHandleCreated || !Visible)
                    return;

                var si = new NativeMethods.SCROLLINFO
                {
                    cbSize = Marshal.SizeOf(typeof(NativeMethods.SCROLLINFO)),
                    fMask = NativeMethods.SIF_ALL | NativeMethods.SIF_DISABLENOSCROLL,
                    nMin = 0,
                    nMax = Maximum,
                    nPage = LargeChange,
                    nPos = Value,
                    nTrackPos = 0
                };

                NativeMethods.SetScrollInfo(new HandleRef(_parentControl, _parentControl.Handle), _orientation, si, true);
            }
        }
    }
}
