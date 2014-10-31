using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GdiPresentation
{
    internal class ElementHost : ContentElement
    {
        private readonly Dictionary<Element, bool> _mouseOver = new Dictionary<Element, bool>();

        private Element _capturedElement;
        private readonly LayoutManager _layoutManager;
        private Element _focusedElement;
        private Element _keyboardFocusedElement;

        public ElementControl Control { get; private set; }

        internal override LayoutManager LayoutManager
        {
            get { return _layoutManager; }
        }

        public Element CapturedElement
        {
            get
            {
                if (Control.Capture)
                    return _capturedElement;
                else
                    return null;
            }
            set
            {
                if (_capturedElement != value)
                {
                    var capturedElement = _capturedElement;

                    _capturedElement = value;
                    Control.Capture = value != null;

                    if (capturedElement != null)
                        capturedElement.RaiseMouseCaptureChanged();
                }
            }
        }

        public Element FocusedElement
        {
            get { return _focusedElement; }
            internal set
            {
                if (_focusedElement != value)
                {
                    var previousFocus = _focusedElement;

                    if (_focusedElement != null)
                        _focusedElement.RaiseLostFocus();

                    _focusedElement = value;

                    if (_focusedElement != null)
                    {
                        // Control hosts have real focus. This means that when
                        // we move away from a control host, we need to put
                        // focus back on ourselves.

                        bool wasControlHost = previousFocus is IControlHost;
                        bool isControlHost = _focusedElement is IControlHost;

                        if (wasControlHost)
                        {
                            // Disable the OnGotFocus functionality.
                            Control.LastFocusedElement = null;
                        }

                        var focusControl =
                            isControlHost
                            ? ((IControlHost)_focusedElement).Control
                            : Control;

                        if (!focusControl.Focused)
                            focusControl.Focus();

                        _focusedElement.ScrollIntoView();
                        _focusedElement.RaiseGotFocus();
                    }
                }

                Control.LastFocusedElement = null;
            }
        }

        public Element KeyboardFocusedElement
        {
            get { return _keyboardFocusedElement; }
            internal set
            {
                if (_keyboardFocusedElement != value)
                {
                    if (_keyboardFocusedElement != null)
                        _keyboardFocusedElement.RaiseLostKeyboardFocus();

                    var oldFocused = _keyboardFocusedElement;
                    _keyboardFocusedElement = value;

                    if (_keyboardFocusedElement != null)
                        _keyboardFocusedElement.RaiseGotKeyboardFocus();

                    UpdateKeyboardFocusWithin(oldFocused, _keyboardFocusedElement);
                }
            }
        }

        private void UpdateKeyboardFocusWithin(Element from, Element to)
        {
            // No change in keyboard focus means we don't have to update
            // the keyboard focus within.

            if (from == to)
                return;

            if (to == null)
            {
                // If keyboard focus is just lost, we just have to remove all
                // keyboard focus within's.

                while (from != null)
                {
                    Debug.Assert(from.IsKeyboardFocusWithin);

                    from.IsKeyboardFocusWithin = false;

                    from = from.Parent;
                }
            }
            else if (from == null)
            {
                // If we only set the keyboard focus, we just have to set all
                // keyboard focused within's.

                while (to != null)
                {
                    Debug.Assert(!to.IsKeyboardFocusWithin);

                    to.IsKeyboardFocusWithin = true;

                    to = to.Parent;
                }
            }
            else
            {
                // If the focus changed (i.e. we got and we have a keyboard
                // focus), we only need to change the within's up until a common
                // ancestor. From that point on, they will have been true and
                // will stay true.

                var ancestor = FindCommonAncestor(from, to);

                while (from != ancestor)
                {
                    Debug.Assert(from.IsKeyboardFocusWithin);

                    from.IsKeyboardFocusWithin = false;

                    from = from.Parent;
                }

                while (to != ancestor)
                {
                    Debug.Assert(!to.IsKeyboardFocusWithin);

                    to.IsKeyboardFocusWithin = true;

                    to = to.Parent;
                }
            }
        }

        private Element FindCommonAncestor(Element a, Element b)
        {
            while (a != null)
            {
                var thisB = b;

                while (thisB != null)
                {
                    if (thisB == a)
                        return thisB;

                    thisB = thisB.Parent;
                }

                a = a.Parent;
            }

            throw new ArgumentException("Elements do not have a common ancestor");
        }

        public ElementHost(ElementControl control)
        {
            if (control == null)
                throw new ArgumentNullException("control");

            Control = control;
            Control.MouseCaptureChanged += (s, e) => CapturedElement = null;

            _layoutManager = new LayoutManager(this);
        }

        internal override void RaiseMouseMove(MouseEventArgs e)
        {
            PrepareMouseLeave();

            try
            {
                var capturedElement = CapturedElement;

                if (capturedElement != null)
                    capturedElement.RaiseMouseMove(e);
                else
                    base.RaiseMouseMove(e);
            }
            finally
            {
                ProcessMouseLeave();
            }
        }

        internal override void RaiseMouseUp(MouseEventArgs e)
        {
            var capturedElement = CapturedElement;

            if (capturedElement != null)
                capturedElement.RaiseMouseUp(e);
            else
                base.RaiseMouseUp(e);

            CapturedElement = null;
        }

        internal override void RaiseMouseDown(MouseEventArgs e)
        {
            var capturedElement = CapturedElement;

            if (capturedElement != null)
                capturedElement.RaiseMouseDown(e);
            else
                base.RaiseMouseDown(e);
        }

        internal override void RaiseMouseDoubleClick(MouseEventArgs e)
        {
            var capturedElement = CapturedElement;

            if (capturedElement != null)
                capturedElement.RaiseMouseDoubleClick(e);
            else
                base.RaiseMouseDoubleClick(e);
        }

        public void RegisterMouseOver(Element element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            _mouseOver[element] = true;
        }

        public void PrepareMouseLeave()
        {
            foreach (var item in new List<Element>(_mouseOver.Keys))
            {
                _mouseOver[item] = false;
            }
        }

        public void ProcessMouseLeave()
        {
            var toRemove = new List<Element>();

            foreach (var item in _mouseOver)
            {
                if (!item.Value)
                {
                    item.Key.RaiseMouseLeave(new ElementEventArgs());
                    toRemove.Add(item.Key);
                }
            }

            foreach (var item in toRemove)
            {
                _mouseOver.Remove(item);
            }
        }

        public Point ScrollOffset
        {
            get
            {
                var displayRect = Control.DisplayRectangle;

                return new Point(
                    -displayRect.X,
                    -displayRect.Y
                );
            }
            set
            {
                Control.SetDisplayRectangleLocation(new System.Drawing.Point(
                    -value.X,
                    -value.Y
                ));
            }
        }
    }
}
