using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GdiPresentation;

namespace GdiPresentation
{
    public abstract class ControlHost<T> : Element, IDisposable, IControlHost
        where T : Control
    {
        private ElementControl _lastOwner;
        private Point _lastScrollOffset;
        private bool _disposed;
        private Size? _preferredSize;
        private bool _inTree;
        private bool _pendingFocus;

        Control IControlHost.Control
        {
            get { return Control; }
        }

        protected ControlHost(T control)
        {
            if (control == null)
                throw new ArgumentNullException("control");

            Control = control;

            // Disable TabStop to disable normal dialog key processing.
            Control.TabStop = false;

            Focusable = true;

            if (!ControlUtil.GetIsInDesignMode(Control))
            {
                Control.Font = SystemFonts.MessageBoxFont;
                Control.GotFocus += Control_GotFocus;
                Control.LostFocus += Control_LostFocus;
                Control.HandleCreated += Control_HandleCreated;
                Control.ParentChanged += Control_ParentChanged;
            }
        }

        void Control_GotFocus(object sender, EventArgs e)
        {
            IsFocused = true;
            IsKeyboardFocused = true;
        }

        void Control_LostFocus(object sender, EventArgs e)
        {
            IsKeyboardFocused = false;
            IsFocused = false;
        }

        void Control_ParentChanged(object sender, EventArgs e)
        {
            if (_pendingFocus)
            {
                _pendingFocus = false;

                Control.Focus();
            }
        }

        void Control_HandleCreated(object sender, EventArgs e)
        {
            if (_pendingFocus)
            {
                _pendingFocus = false;

                Control.Parent.BeginInvoke(new Func<bool>(Control.Focus));
            }
        }

        public T Control { get; private set; }

        protected override Size MeasureOverride(Size desiredSize)
        {
            if (!_preferredSize.HasValue)
                _preferredSize = (Size)Control.GetPreferredSize((System.Drawing.Size)desiredSize);

            return _preferredSize.Value;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (!_inTree)
                return finalSize;

            if (LayoutManager != null)
            {
                if (_lastOwner != LayoutManager.Host.Control)
                {
                    if (_lastOwner != null)
                        _lastOwner.Controls.Remove(Control);

                    _lastOwner = LayoutManager.Host.Control;

                    _lastOwner.Controls.Add(Control);
                }

                _lastScrollOffset  = ElementUtil.GetScrollOffset(this);

                var offset = TransformPointTo(Point.Empty, LayoutManager.Host);

                Control.SetBounds(
                    offset.X - _lastScrollOffset.X,
                    offset.Y - _lastScrollOffset.Y,
                    finalSize.Width,
                    finalSize.Height
                );
            }

            return (Size)Control.Size;
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            // Setting focus on the control early is tricky. The problem is
            // that it takes quite a while for the control to be displayed
            // (not until the next paint).

            // Because of this, we handle this case special. When the handle
            // hasn't been created (i.e. the control hasn't been added to
            // the parent Controls collection yet), we have to do a two step
            // delay to be able to set focus. First, we must wait for the
            // HandleCreated event (forcing creation of the handle doesn't
            // help). Then, we need to do a second step through BeginInvoke.
            // This is triggered by setting _pendingFocus to true.

            if (Control.IsHandleCreated)
                Control.Focus();
            else
                _pendingFocus = true;
        }

        protected override void OnLoaded(EventArgs e)
        {
            base.OnLoaded(e);

            _inTree = true;
        }

        protected override void OnUnloaded(EventArgs e)
        {
            base.OnUnloaded(e);

            _inTree = false;

            if (_lastOwner != null)
                _lastOwner.Controls.Remove(Control);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Control.Dispose();

                _disposed = true;
            }
        }
    }
}
