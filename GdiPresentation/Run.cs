using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GdiPresentation
{
    public class Run
    {
        private static readonly string _defaultFontFamily;
        private static readonly FontStyle _defaultFontStyle;
        private static readonly float _defaultFontSize;

        static Run()
        {
            var font = SystemFonts.MessageBoxFont;

            _defaultFontFamily = font.Name;
            _defaultFontStyle = (FontStyle)font.Style;
            _defaultFontSize = font.Size;
        }

        private string _text;
        private Color? _foreColor;
        private string _fontFamily;
        private FontStyle? _fontStyle;
        private float? _fontSize;

        public object Tag { get; set; }

        internal int EllipsisSize { get; set; }

        internal WordCache[] WordCache { get; set; }

        public TextBlock Parent { get; internal set; }

        public bool Capture
        {
            get { return Parent != null && Parent.CapturedRun == this; }
            set
            {
                if (Parent != null)
                    Parent.CapturedRun = value ? this : null;
            }
        }

        public string Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    InvalidateMeasure();
                }
            }
        }

        public Color? ForeColor
        {
            get { return _foreColor; }
            set
            {
                if (_foreColor != value)
                {
                    _foreColor = value;
                    Invalidate();
                }
            }
        }

        public string FontFamily
        {
            get { return _fontFamily; }
            set
            {
                if (_fontFamily != value)
                {
                    _fontFamily = value;
                    InvalidateMeasure();
                }
            }
        }

        public FontStyle? FontStyle
        {
            get { return _fontStyle; }
            set
            {
                if (_fontStyle != value)
                {
                    _fontStyle = value;
                    InvalidateMeasure();
                }
            }
        }

        public float? FontSize
        {
            get { return _fontSize; }
            set
            {
                if (_fontSize != value)
                {
                    _fontSize = value;
                    InvalidateMeasure();
                }
            }
        }

        public bool IsMouseOver { get; private set; }

        public bool IsMouseDirectlyOver { get; private set; }

        public Cursor Cursor { get; set; }

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

        public event ElementEventHandler MouseEnter;

        protected virtual void OnMouseEnter(ElementEventArgs e)
        {
            var ev = MouseEnter;

            if (ev != null)
                ev(this, e);
        }

        public event ElementEventHandler MouseLeave;

        protected virtual void OnMouseLeave(ElementEventArgs e)
        {
            var ev = MouseLeave;

            if (ev != null)
                ev(this, e);
        }

        public Run()
        {
        }

        public Run(string text)
        {
            Text = text;
        }

        internal Font GetFont()
        {
            string fontFamily = FontFamily;

            if (fontFamily == null && Parent != null)
                fontFamily = Parent.FontFamily;

            if (fontFamily == null)
                fontFamily = _defaultFontFamily;

            var fontStyle = FontStyle;

            if (!fontStyle.HasValue && Parent != null)
                fontStyle = Parent.FontStyle;

            if (!fontStyle.HasValue)
                fontStyle = _defaultFontStyle;

            float? fontSize = FontSize;

            if (!fontSize.HasValue && Parent != null)
                fontSize = Parent.FontSize;

            if (!fontSize.HasValue)
                fontSize = _defaultFontSize;

            return FontCacheManager.Current.GetFont(fontFamily, fontSize.Value, fontStyle.Value);
        }

        internal void RaiseMouseMove(MouseEventArgs e, bool directlyOver)
        {
            IsMouseDirectlyOver = directlyOver;

            if (!IsMouseOver)
            {
                IsMouseOver = true;

                RaiseMouseEnter(new ElementEventArgs());
            }

            Parent?.RegisterMouseOver(this);

            OnMouseMove(e);
        }

        internal void RaiseMouseUp(MouseEventArgs e)
        {
            OnMouseUp(e);
        }

        internal void RaiseMouseDown(MouseEventArgs e)
        {
            OnMouseDown(e);
        }

        internal void RaiseMouseEnter(ElementEventArgs e)
        {
            OnMouseEnter(e);
        }

        internal void RaiseMouseLeave(ElementEventArgs e)
        {
            IsMouseOver = false;
            IsMouseDirectlyOver = false;

            OnMouseLeave(e);
        }

        public void InvalidateMeasure()
        {
            WordCache = null;

            if (Parent != null)
                Parent.InvalidateMeasure();
        }

        public void InvalidateArrange()
        {
            if (Parent != null)
                Parent.InvalidateArrange();
        }

        public void Invalidate()
        {
            if (Parent != null)
                Parent.Invalidate();
        }

        internal void RaiseResolveCursor(ResolveCursorEventArgs e)
        {
            if (Cursor != null)
            {
                e.Cursor = Cursor;
                e.PreventBubble();
            }
        }
    }
}
