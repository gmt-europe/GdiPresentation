using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace GdiPresentation
{
    public abstract partial class Element
    {
        private AttachedProperties _attachedProperties;
        private Thickness _margin;
        private Brush _background;
        private int _minHeight;
        private int _maxHeight;
        private int _minWidth;
        private int _maxWidth;
        private HorizontalAlignment _horizontalAlignment;
        private VerticalAlignment _verticalAlignment;
        private Size _previousAvailableSize;
        private Element _parent;
        private Visibility _visibility;
        private bool _neverPainted = true;
        private bool _isKeyboardFocusWithin;
        private bool _focusable;
        private int? _width;
        private int? _height;
        private bool _isMouseDirectlyOver;

        internal virtual LayoutManager LayoutManager { get; private set; }

        internal Rect PreviousFinalRect { get; private set; }

        public bool IsMeasureValid { get; private set; }

        public bool IsArrangeValid { get; private set; }

        public object Tag { get; set; }

        public Element Parent
        {
            get { return _parent; }
            internal set
            {
                if (_parent != value)
                {
                    var oldParent = _parent;

                    _parent = value;

                    if (oldParent != null)
                        RaiseUnloaded();

                    // Only raise the loaded event if we're actually in a tree.

                    if (_parent != null && _parent.LayoutManager != null)
                        RaiseLoaded();
                }
            }
        }

        public Thickness Margin
        {
            get { return _margin; }
            set
            {
                if (_margin != value)
                {
                    _margin = value;
                    InvalidateMeasure();
                }
            }
        }

        public Brush Background
        {
            get { return _background; }
            set
            {
                if (_background != value)
                {
                    _background = value;
                    Invalidate();
                }
            }
        }

        public int MinHeight
        {
            get { return _minHeight; }
            set
            {
                if (_minHeight != value)
                {
                    _minHeight = value;
                    InvalidateMeasure();
                }
            }
        }

        public int MaxHeight
        {
            get { return _maxHeight; }
            set
            {
                if (_maxHeight != value)
                {
                    _maxHeight = value;
                    InvalidateMeasure();
                }
            }
        }

        public int? Width
        {
            get { return _width; }
            set
            {
                if (_width != value)
                {
                    _width = value;
                    InvalidateMeasure();
                }
            }
        }

        public int? Height
        {
            get { return _height; }
            set
            {
                if (_height != value)
                {
                    _height = value;
                    InvalidateMeasure();
                }
            }
        }

        public int MinWidth
        {
            get { return _minWidth; }
            set
            {
                if (_minWidth != value)
                {
                    _minWidth = value;
                    InvalidateMeasure();
                }
            }
        }

        public int MaxWidth
        {
            get { return _maxWidth; }
            set
            {
                if (_maxWidth != value)
                {
                    _maxWidth = value;
                    InvalidateMeasure();
                }
            }
        }

        public HorizontalAlignment HorizontalAlignment
        {
            get { return _horizontalAlignment; }
            set
            {
                if (_horizontalAlignment != value)
                {
                    _horizontalAlignment = value;
                    InvalidateMeasure();
                }
            }
        }

        public VerticalAlignment VerticalAlignment
        {
            get { return _verticalAlignment; }
            set
            {
                if (_verticalAlignment != value)
                {
                    _verticalAlignment = value;
                    InvalidateMeasure();
                }
            }
        }

        public bool IsMouseOver { get; private set; }

        public bool IsMouseDirectlyOver
        {
            get { return _isMouseDirectlyOver; }
            private set
            {
                if (_isMouseDirectlyOver != value)
                {
                    _isMouseDirectlyOver = value;
                    OnIsMouseDirectlyOverChanged(EventArgs.Empty);
                }
            }
        }

        public event EventHandler IsMouseDirectlyOverChanged;

        protected virtual void OnIsMouseDirectlyOverChanged(EventArgs e)
        {
            var ev = IsMouseDirectlyOverChanged;
            if (ev != null)
                ev(this, e);
        }

        public int ActualHeight
        {
            get { return Size.Height; }
        }

        public int ActualWidth
        {
            get { return Size.Width; }
        }

        public Size Size { get; private set; }

        public Vector Offset { get; private set; }

        public Size DesiredSize { get; private set; }

        public bool Capture
        {
            get
            {
                return LayoutManager != null && LayoutManager.Host.CapturedElement == this;
            }
            set
            {
                if (LayoutManager != null)
                    LayoutManager.Host.CapturedElement = value ? this : null;
            }
        }

        public Cursor Cursor { get; set; }

        public Visibility Visibility
        {
            get { return _visibility; }
            set
            {
                if (_visibility != value)
                {
                    bool hadSize = _visibility != Visibility.Collapsed;

                    _visibility = value;

                    bool haveSize = _visibility != Visibility.Collapsed;

                    if (hadSize == haveSize)
                    {
                        Invalidate();
                    }
                    else
                    {
                        if (hadSize)
                        {
                            if (Parent != null)
                                Parent.Invalidate(new Rect(Offset, Size));
                        }

                        InvalidateMeasure();
                    }
                }
            }
        }

        public bool Focusable
        {
            get { return _focusable; }
            set
            {
                if (_focusable != value)
                {
                    _focusable = value;

                    if (!value)
                        IsFocused = false;
                }
            }
        }

        public bool IsFocused
        {
            get
            {
                return LayoutManager != null && LayoutManager.Host.FocusedElement == this;
            }
            protected set
            {
                if (LayoutManager == null)
                    return;

                if (value)
                    LayoutManager.Host.FocusedElement = this;
                else if (LayoutManager.Host.FocusedElement == this)
                    LayoutManager.Host.FocusedElement = null;
            }
        }

        public bool IsKeyboardFocused
        {
            get { return LayoutManager.Host.KeyboardFocusedElement == this; }
            protected set
            {
                if (LayoutManager == null)
                    return;

                if (value)
                    LayoutManager.Host.KeyboardFocusedElement = this;
                else if (LayoutManager.Host.KeyboardFocusedElement == this)
                    LayoutManager.Host.KeyboardFocusedElement = null;
            }
        }

        public bool IsKeyboardFocusWithin
        {
            get { return _isKeyboardFocusWithin; }
            internal set
            {
                if (_isKeyboardFocusWithin != value)
                {
                    _isKeyboardFocusWithin = value;
                    OnIsKeyboardFocusWithinChanged(EventArgs.Empty);
                }
            }
        }

        public event ElementPaintEventHandler Paint;

        protected virtual void OnPaint(ElementPaintEventArgs e)
        {
            var ev = Paint;
            if (ev != null)
                ev(this, e);
        }

        public event MouseEventHandler MouseMove;

        protected virtual void OnMouseMove(MouseEventArgs e)
        {
            var ev = MouseMove;
            if (ev != null)
                ev(this, e);
        }

        public event MouseEventHandler MouseUp;

        protected virtual void OnMouseUp(MouseEventArgs e)
        {
            var ev = MouseUp;
            if (ev != null)
                ev(this, e);
        }

        public event MouseEventHandler MouseDown;

        protected virtual void OnMouseDown(MouseEventArgs e)
        {
            var ev = MouseDown;
            if (ev != null)
                ev(this, e);
        }

        public event MouseEventHandler MouseDoubleClick;

        protected virtual void OnMouseDoubleClick(MouseEventArgs e)
        {
            var ev = MouseDoubleClick;
            if (ev != null)
                ev(this, e);
        }

        public event EventHandler MouseEnter;

        protected virtual void OnMouseEnter(EventArgs e)
        {
            var ev = MouseEnter;
            if (ev != null)
                ev(this, e);
        }

        public event EventHandler MouseLeave;

        protected virtual void OnMouseLeave(EventArgs e)
        {
            var ev = MouseLeave;
            if (ev != null)
                ev(this, e);
        }

        public event EventHandler Loaded;

        protected virtual void OnLoaded(EventArgs e)
        {
            var ev = Loaded;
            if (ev != null)
                ev(this, e);
        }

        public event EventHandler Unloaded;

        protected virtual void OnUnloaded(EventArgs e)
        {
            var ev = Unloaded;
            if (ev != null)
                ev(this, e);
        }

        public event EventHandler GotFocus;

        protected virtual void OnGotFocus(EventArgs e)
        {
            var ev = GotFocus;
            if (ev != null)
                ev(this, e);
        }

        internal void RaiseGotFocus()
        {
            OnGotFocus(EventArgs.Empty);
        }

        public event EventHandler LostFocus;

        protected virtual void OnLostFocus(EventArgs e)
        {
            var ev = LostFocus;
            if (ev != null)
                ev(this, e);
        }

        internal void RaiseLostFocus()
        {
            OnLostFocus(EventArgs.Empty);
        }

        public event EventHandler GotKeyboardFocus;

        protected virtual void OnGotKeyboardFocus(EventArgs e)
        {
            var ev = GotKeyboardFocus;
            if (ev != null)
                ev(this, e);
        }

        internal void RaiseGotKeyboardFocus()
        {
            OnGotKeyboardFocus(EventArgs.Empty);
        }

        public event EventHandler LostKeyboardFocus;

        protected virtual void OnLostKeyboardFocus(EventArgs e)
        {
            var ev = LostKeyboardFocus;
            if (ev != null)
                ev(this, e);
        }

        internal void RaiseLostKeyboardFocus()
        {
            OnLostKeyboardFocus(EventArgs.Empty);
        }

        public event EventHandler IsKeyboardFocusWithinChanged;

        protected virtual void OnIsKeyboardFocusWithinChanged(EventArgs e)
        {
            var ev = IsKeyboardFocusWithinChanged;
            if (ev != null)
                ev(this, e);
        }

        public event EventHandler MouseCaptureChanged;

        protected virtual void OnMouseCaptureChanged(EventArgs e)
        {
            var ev = MouseCaptureChanged;
            if (ev != null)
                ev(this, EventArgs.Empty);
        }

        internal void RaiseMouseCaptureChanged()
        {
            OnMouseCaptureChanged(EventArgs.Empty);
        }

        protected Element()
        {
            MaxHeight = int.MaxValue;
            MaxWidth = int.MaxValue;
        }

        public void Measure(Size availableSize)
        {
            Measure(availableSize, true);
        }

        private void Measure(Size availableSize, bool invalidateArrange)
        {
            if (IsMeasureValid && _previousAvailableSize == availableSize)
                return;

            IsMeasureValid = true;
            _previousAvailableSize = availableSize;

            if (Visibility == Visibility.Collapsed)
            {
                DesiredSize = Size.Empty;
                return;
            }

            if (invalidateArrange)
                InvalidateArrange(false);

            int width =
                Width.HasValue
                ? Width.Value
                : Math.Min(Math.Max(availableSize.Width - Margin.Horizontal, MinWidth), MaxWidth);

            int height =
                Height.HasValue
                ? Height.Value
                : Math.Min(Math.Max(availableSize.Height - Margin.Vertical, MinHeight), MaxHeight);

            var size = MeasureOverride(new Size(width, height));

            width =
                Width.HasValue
                ? Width.Value
                : Math.Min(Math.Max(size.Width, MinWidth), MaxWidth);

            height =
                Height.HasValue
                ? Height.Value
                : Math.Min(Math.Max(size.Height, MinHeight), MaxHeight);

            DesiredSize = new Size(
                width + Margin.Horizontal,
                height + Margin.Vertical
            );
        }

        protected virtual Size MeasureOverride(Size desiredSize)
        {
            return new Size(0, 0);
        }

        public void Arrange(Rect finalRect)
        {
            Arrange(finalRect, true);
        }

        internal void Arrange(Rect finalRect, bool invalidatePaint)
        {
            if (IsArrangeValid && finalRect == PreviousFinalRect)
                return;

            IsArrangeValid = true;
            PreviousFinalRect = finalRect;

            if (Visibility == Visibility.Collapsed)
            {
                Offset = Vector.Empty;
                Size = Size.Empty;
                return;
            }

            // If we're arranging with dimensions different from what we've
            // measured with, we do another measure pass to give the element
            // a chance to re-adjust. Since we don't have a real int.Infinite,
            // we use the surrogate IsInfinite from IntUtil.

            if (
                (!IntUtil.IsInfinite(_previousAvailableSize.Width) && _previousAvailableSize.Width != finalRect.Width) ||
                (!IntUtil.IsInfinite(_previousAvailableSize.Height) && _previousAvailableSize.Height != finalRect.Height)
            )
                Measure(finalRect.Size, false);

            int left = finalRect.Left + Margin.Left;
            int top = finalRect.Top + Margin.Top;
            int finalWidth = finalRect.Width - Margin.Horizontal;
            int finalHeight = finalRect.Height - Margin.Vertical;
            int width;
            int height;

            var horizontalAlignment = HorizontalAlignment;
            var verticalAlignment = VerticalAlignment;

            if (Width.HasValue)
            {
                width = Width.Value;
            }
            else
            {
                width = DesiredSize.Width - Margin.Horizontal;

                if (horizontalAlignment == HorizontalAlignment.Stretch)
                {
                    width = Math.Max(width, finalWidth);
                    horizontalAlignment = HorizontalAlignment.Center;
                }

                width = Math.Min(Math.Max(width, MinWidth), MaxWidth);
            }

            if (Height.HasValue)
            {
                height = Height.Value;
            }
            else
            {
                height = DesiredSize.Height - Margin.Vertical;

                if (verticalAlignment == VerticalAlignment.Stretch)
                {
                    height = Math.Max(height, finalHeight);
                    verticalAlignment = VerticalAlignment.Middle;
                }

                height = Math.Min(Math.Max(height, MinHeight), MaxHeight);
            }

            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Center:
                    left += (finalWidth - width) / 2;
                    break;

                case HorizontalAlignment.Right:
                    left += finalWidth - width;
                    break;
            }

            switch (verticalAlignment)
            {
                case VerticalAlignment.Middle:
                    top += (finalHeight - height) / 2;
                    break;

                case VerticalAlignment.Bottom:
                    top += finalHeight - height;
                    break;
            }

            var oldOffset = Offset;
            var oldSize = Size;

            Offset = new Vector(left, top);
            Size = new Size(width, height);

            ArrangeOverride(Size);

            if (
                invalidatePaint &&
                (Offset != oldOffset || Size != oldSize)
            ) {
                if (Parent != null)
                    Parent.Invalidate(new Rect(oldOffset, oldSize));

                Invalidate();
            }
        }

        protected virtual Size ArrangeOverride(Size finalSize)
        {
            return finalSize;
        }

        internal void RaisePaint(PaintEventArgs e)
        {
            _neverPainted = false;

            if (Visibility != Visibility.Visible)
                return;

            Debug.Assert(LayoutManager != null);

            if (LayoutManager == null)
                return;

            var bounds = new Rect(TransformPointTo(Point.Empty, LayoutManager.Host), Size);

            if (!e.ClipRectangle.IntersectsWith(bounds))
                return;

            Region currentClip = null;

            try
            {
                if (Parent != null)
                    currentClip = ClipToParent(e);

                var paintEventArgs = new ElementPaintEventArgs(e.Graphics, e.ClipRectangle, bounds);

                OnPaintBackground(paintEventArgs);
                
                for (int i = 0, count = GetChildrenCount(); i < count; i++)
                {
                    GetChild(i).RaisePaint(e);
                }

                OnPaint(paintEventArgs);
            }
            finally
            {
                if (currentClip != null)
                    e.Graphics.Clip = currentClip;
            }
        }

        public event ElementPaintEventHandler PaintBackground;

        protected virtual void OnPaintBackground(ElementPaintEventArgs e)
        {
            if (Background == null || Background == Brush.Transparent)
                return;

            using (var brush = Background.CreateBrush(e.Bounds))
            {
                e.Graphics.FillRectangle(brush, (Rectangle)e.Bounds);
            }

            var ev = PaintBackground;
            if (ev != null)
                ev(this, e);
        }

        private Region ClipToParent(PaintEventArgs e)
        {
            Region currentClip = null;

            var location = TransformPointTo(Point.Empty, Parent);

            bool performClip = location.X < 0 || location.Y < 0;

            if (!performClip && Parent != null)
            {
                performClip =
                    Width + location.X > Parent.Size.Width ||
                    Height + location.Y > Parent.Size.Height;
            }

            if (performClip)
            {
                currentClip = e.Graphics.Clip;

                e.Graphics.SetClip(new Rectangle(
                    (System.Drawing.Point)Parent.TransformPointTo(Point.Empty, LayoutManager.Host),
                    (System.Drawing.Size)Parent.Size
                ));
            }

            return currentClip;
        }

        internal virtual void RaiseMouseMove(MouseEventArgs e)
        {
            Debug.Assert(LayoutManager != null);

            if (LayoutManager == null)
                return;

            if (!IsMouseOver)
            {
                IsMouseOver = true;

                RaiseMouseEnter(new ElementEventArgs());
            }

            LayoutManager.Host.RegisterMouseOver(this);

            Bubble(e, (s, ea) => s.RaiseMouseMove(e), BubbleDirection.Inverse);

            if (e.IsMouseDirectlyOverAssigned)
            {
                IsMouseDirectlyOver = false;
            }
            else
            {
                var location = e.GetPosition(this);

                IsMouseDirectlyOver =
                    Background != null &&
                    location.X >= 0 &&
                    location.Y >= 0 &&
                    location.X <= ActualWidth &&
                    location.Y <= ActualHeight;

                if (IsMouseDirectlyOver)
                    e.IsMouseDirectlyOverAssigned = true;
            }

            if (!e.IsBubblePrevented && Background != null)
                OnMouseMove(e);
        }

        internal virtual void RaiseMouseUp(MouseEventArgs e)
        {
            Bubble(e, (s, ea) => s.RaiseMouseUp(e), BubbleDirection.Inverse);

            if (!e.IsBubblePrevented && Background != null)
                OnMouseUp(e);
        }

        internal virtual void RaiseMouseDown(MouseEventArgs e)
        {
            Bubble(e, (s, ea) => s.RaiseMouseDown(e), BubbleDirection.Inverse);

            if (Focusable && !e.FocusChanged)
            {
                e.FocusChanged = true;
                Focus();
            }

            if (!e.IsBubblePrevented && Background != null)
                OnMouseDown(e);
        }

        internal virtual void RaiseMouseDoubleClick(MouseEventArgs e)
        {
            Bubble(e, (s, ea) => s.RaiseMouseDoubleClick(e), BubbleDirection.Inverse);

            if (Focusable && !e.FocusChanged)
            {
                e.FocusChanged = true;
                Focus();
            }

            if (!e.IsBubblePrevented && Background != null)
                OnMouseDoubleClick(e);
        }

        internal virtual void RaiseResolveCursor(ResolveCursorEventArgs e)
        {
            Bubble(e, (s, ea) => s.RaiseResolveCursor(e), BubbleDirection.Inverse);

            if (!e.IsBubblePrevented && Cursor != null)
            {
                e.Cursor = Cursor;
                e.PreventBubble();
            }
        }

        private void Bubble(MouseEventArgs e, Action<Element, MouseEventArgs> action, BubbleDirection direction)
        {
            for (int i = 0, count = GetChildrenCount(); i < count; i++)
            {
                var child = GetChild(
                    direction == BubbleDirection.Normal
                    ? i
                    : count - (i + 1)
                );

                if (child.Visibility != Visibility.Visible)
                    continue;

                if (IsMouseOverCore(e, child))
                    action(child, e);

                if (e.IsBubblePrevented)
                    return;
            }
        }

        internal virtual bool IsMouseOverCore(MouseEventArgs e, Element child)
        {
            var location = e.GetPosition(child);

            return
                location.X >= 0 &&
                location.Y >= 0 &&
                location.X <= child.ActualWidth &&
                location.Y <= child.ActualHeight;
        }

        public Element GetChildAtPoint(Point location)
        {
            return GetChildAtPoint(location, GetChildAtPointSkip.None);
        }

        public Element GetChildAtPoint(Point location, GetChildAtPointSkip skip)
        {
            for (int i = GetChildrenCount() - 1; i >= 0; i--)
            {
                var child = GetChild(i);

                var translated = child.TranslateLocation(location);

                if (
                    !ShouldSkip(child, skip) &&
                    translated.X >= 0 &&
                    translated.Y >= 0 &&
                    translated.X <= child.ActualWidth &&
                    translated.Y <= child.ActualHeight
                )
                    return child.GetChildAtPoint(location, skip) ?? child;
            }

            return null;
        }

        private bool ShouldSkip(Element child, GetChildAtPointSkip skip)
        {
            switch (skip)
            {
                case GetChildAtPointSkip.Invisible: return child.Visibility != Visibility.Visible;
                case GetChildAtPointSkip.Transparent: return child.Background == Brush.Transparent;
                default: return false;
            }
        }

        internal virtual void RaiseMouseEnter(EventArgs e)
        {
            if (Background != null)
                OnMouseEnter(e);
        }

        internal virtual void RaiseMouseLeave(EventArgs e)
        {
            IsMouseOver = false;
            IsMouseDirectlyOver = false;

            if (Background != null)
                OnMouseLeave(e);
        }

        internal void InvalidateMeasure()
        {
            InvalidateMeasure(true);
        }

        internal void InvalidateMeasure(bool invalidatePaint)
        {
            if (IsMeasureValid)
            {
                InvalidateMeasureInternal();

                if (LayoutManager != null)
                    LayoutManager.InvalidateMeasure(this);

                if (invalidatePaint)
                    Invalidate();
            }
        }

        internal void InvalidateMeasureInternal()
        {
            IsMeasureValid = false;
        }

        internal void InvalidateArrange()
        {
            InvalidateArrange(true);
        }

        internal void InvalidateArrange(bool invalidatePaint)
        {
            if (!IsArrangeValid)
                return;

            InvalidateArrangeInternal();

            if (LayoutManager != null)
                LayoutManager.InvalidateArrange(this);

            if (invalidatePaint)
                Invalidate();
        }

        internal void InvalidateArrangeInternal()
        {
            IsArrangeValid = false;
        }

        public void Invalidate()
        {
            if (LayoutManager == null || _neverPainted)
                return;

            LayoutManager.PaintElementQueue.Add(this);
        }

        internal void Invalidate(Rect bounds)
        {
            if (LayoutManager == null || _neverPainted)
                return;

            LayoutManager.PaintBoundsQueue.Add(new Rect(
                TransformPointTo(bounds.Location, LayoutManager.Host),
                bounds.Size
            ));
        }

        [DebuggerStepThrough]
        public virtual int GetChildrenCount()
        {
            return 0;
        }

        [DebuggerStepThrough]
        public virtual Element GetChild(int index)
        {
            return null;
        }

        internal object GetAttachedValue(object key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            if (_attachedProperties == null)
                return null;

            return _attachedProperties.Find(key);
        }

        internal void SetAttachedValue(object key, object value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            if (value == null)
            {
                if (_attachedProperties != null)
                    _attachedProperties.Remove(key);
            }
            else
            {
                if (_attachedProperties == null)
                    _attachedProperties = new AttachedProperties();

                _attachedProperties.Set(key, value);
            }
        }

        private void RaiseLoaded()
        {
            LayoutManager = Parent.LayoutManager;

            // Reset all measure/arrange/paint state.

            IsMeasureValid = false;
            IsArrangeValid = false;
            _previousAvailableSize = Size.Empty;
            PreviousFinalRect = Rect.Empty;

            InvalidateMeasure();

            OnLoaded(EventArgs.Empty);

            for (int i = 0, count = GetChildrenCount(); i < count; i++)
            {
                GetChild(i).RaiseLoaded();
            }
        }

        private void RaiseUnloaded()
        {
            // Ensure that focus is removed if the current element has focus.

            IsFocused = false;

            LayoutManager = null;

            OnUnloaded(EventArgs.Empty);

            for (int i = 0, count = GetChildrenCount(); i < count; i++)
            {
                GetChild(i).RaiseUnloaded();
            }
        }

        public Point TransformPointTo(Point origin, Element element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            if (element == this)
                return origin;

            var item = this;

            int x = origin.X;
            int y = origin.Y;

            do
            {
                var offset = item.Offset;

                x += offset.X;
                y += offset.Y;

                item = item.Parent;
            }
            while (item != null && item != element);

            if (item == null)
                return Point.Empty;

            return new Point(x, y);
        }

        public void Focus()
        {
            if (Focusable)
                IsFocused = true;
        }

        public Element FindNextElement(bool focusable, bool wrap, bool forward)
        {
            return FindNextElement(focusable, wrap, forward, this, true);
        }

        public Element FindSelfOrNextElement(bool focusable, bool wrap, bool forward)
        {
            return FindNextElement(focusable, wrap, forward, this, false);
        }

        private Element FindNextElement(bool focusable, bool wrap, bool forward, Element element, bool skipSelf)
        {
            foreach (var child in new ElementEnumerable(element, skipSelf, forward, wrap))
            {
                if (child.Focusable || !focusable)
                    return child;
            }

            return null;
        }

        [DebuggerStepThrough]
        internal int IndexOf(Element item)
        {
            for (int i = 0, count = GetChildrenCount(); i < count; i++)
            {
                if (GetChild(i) == item)
                    return i;
            }

            return -1;
        }

        internal Point TranslateLocation(Point location)
        {
            if (LayoutManager == null)
                return Point.Empty;

            var host = LayoutManager.Host;

            var clientLocation = host.Control.PointToClient((System.Drawing.Point)location);

            var offset = TransformPointTo(
                (Point)host.Control.DisplayRectangle.Location,
                host
            );

            return new Point(
                clientLocation.X - offset.X,
                clientLocation.Y - offset.Y
            );
        }

        public void ScrollIntoView()
        {
            if (LayoutManager == null)
                return;

            var topLeft = TransformPointTo(new Point(0, 0), LayoutManager.Host);
            var bottomRight = TransformPointTo(new Point(ActualWidth, ActualHeight), LayoutManager.Host);

            var visibleTopLeft = LayoutManager.Host.ScrollOffset;
            var clientSize = LayoutManager.Host.Control.ClientSize;
            var visibleBottomRight = new Point(
                visibleTopLeft.X + clientSize.Width,
                visibleTopLeft.Y + clientSize.Height
            );

            var staticPanel = LayoutManager.Host.Content as StaticPanel;
            var offset = Vector.Empty;

            if (staticPanel != null && IsOnNormalAligned())
            {
                offset = new Vector(staticPanel.LeftOffset, staticPanel.TopOffset);

                visibleTopLeft = new Point(
                    visibleTopLeft.X + offset.X,
                    visibleTopLeft.Y + offset.Y
                );
            }

            int? newX = null;
            int? newY = null;

            if (topLeft.X < visibleTopLeft.X)
                newX = topLeft.X - offset.X;
            else if (bottomRight.X > visibleBottomRight.X)
                newX = Math.Min(bottomRight.X - clientSize.Width, topLeft.X - offset.X);

            if (topLeft.Y < visibleTopLeft.Y)
                newY = topLeft.Y - offset.Y;
            else if (bottomRight.Y > visibleBottomRight.Y)
                newY = Math.Min(bottomRight.Y - clientSize.Height, topLeft.Y - offset.Y);

            if (newX.HasValue || newY.HasValue)
            {
                var scrollOffset = LayoutManager.Host.ScrollOffset;

                LayoutManager.Host.ScrollOffset = new Point(
                    newX.GetValueOrDefault(scrollOffset.X),
                    newY.GetValueOrDefault(scrollOffset.Y)
                );
            }
        }

        private bool IsOnNormalAligned()
        {
            var element = this;

            while (element != null && !(element.Parent is StaticPanel))
            {
                element = element.Parent;
            }

            if (element == null)
                return false;

            return StaticPanel.GetStaticAlignment(element) == StaticAlignment.Normal;
        }

        private enum BubbleDirection
        {
            Normal,
            Inverse
        }
    }
}
