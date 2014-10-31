using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace GdiPresentation
{
    public class TextBlock : Element
    {
        private readonly Dictionary<Run, bool> _mouseOver = new Dictionary<Run, bool>();
        private readonly TextMeasureCache _cache = new TextMeasureCache();
        private bool _wrap;
        private Color? _foreColor;
        private string _fontFamily;
        private FontStyle? _fontStyle;
        private float? _fontSize;
        private Run _capturedRun;
        private TextTrimming _textTrimming;

        public Run CapturedRun
        {
            get
            {
                if (Capture)
                    return _capturedRun;
                else
                    return null;
            }
            set
            {
                _capturedRun = value;
                Capture = value != null;
            }
        }

        public TextBlock()
        {
            Runs = new RunCollection(this);
            Runs.CollectionChanged += Runs_CollectionChanged;
        }

        public TextBlock(string text)
            : this()
        {
            if (text == null)
                throw new ArgumentNullException("text");

            Runs.Add(new Run(text));
        }

        void Runs_CollectionChanged(object sender, EventArgs e)
        {
            InvalidateMeasure();
        }

        public bool Wrap
        {
            get { return _wrap; }
            set
            {
                if (_wrap != value)
                {
                    _wrap = value;
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

        public RunCollection Runs { get; private set; }

        public TextTrimming TextTrimming
        {
            get { return _textTrimming; }
            set
            {
                if (_textTrimming != value)
                {
                    _textTrimming = value;
                    Invalidate();
                }
            }
        }

        internal override void RaiseMouseMove(MouseEventArgs e)
        {
            PrepareMouseLeave();

            try
            {
                var capturedRun = CapturedRun;

                if (capturedRun != null)
                {
                    bool directlyOver = false;

                    var mouseLocation = e.GetPosition(this);

                    var location = new Point(
                        mouseLocation.X - Offset.X,
                        mouseLocation.Y - Offset.Y
                    );

                    for (int i = 0; i < capturedRun.WordCache.Length; i++)
                    {
                        if (capturedRun.WordCache[i].Bounds.Contains(location))
                        {
                            directlyOver = true;
                            break;
                        }
                    }

                    capturedRun.RaiseMouseMove(e, directlyOver);
                }
                else
                {
                    BubbleEvent(e, (s, ea) => s.RaiseMouseMove(ea, true), base.RaiseMouseMove);
                }
            }
            finally
            {
                ProcessMouseLeave();
            }
        }

        internal override void RaiseMouseUp(MouseEventArgs e)
        {
            var capturedRun = CapturedRun;

            if (capturedRun != null)
                capturedRun.RaiseMouseUp(e);
            else
                BubbleEvent(e, (s, ea) => s.RaiseMouseUp(ea), base.RaiseMouseUp);

            _capturedRun = null;
        }

        internal override void RaiseMouseDown(MouseEventArgs e)
        {
            var capturedRun = CapturedRun;

            if (capturedRun != null)
                capturedRun.RaiseMouseDown(e);
            else
                BubbleEvent(e, (s, ea) => s.RaiseMouseDown(ea), base.RaiseMouseDown);
        }

        internal override void RaiseMouseLeave(EventArgs e)
        {
            PrepareMouseLeave();
            ProcessMouseLeave();

            base.RaiseMouseLeave(e);
        }

        protected override Size MeasureOverride(Size desiredSize)
        {
            _cache.Measure(this);
            _cache.Layout(this, desiredSize, false);

            if (TextTrimming != TextTrimming.None)
            {
                return new Size(
                    Math.Min(desiredSize.Width, _cache.DesiredSize.Width),
                    Math.Min(desiredSize.Height, _cache.DesiredSize.Height)
                );
            }

            return _cache.DesiredSize;
        }

        internal override void RaiseResolveCursor(ResolveCursorEventArgs e)
        {
            BubbleEvent(e, (s, ea) => s.RaiseResolveCursor(ea), base.RaiseResolveCursor);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (finalSize != _cache.DesiredSize)
                _cache.Layout(this, finalSize, true);

            return _cache.DesiredSize;
        }

        protected override void OnPaint(ElementPaintEventArgs e)
        {
            Debug.Assert(LayoutManager != null);

            if (LayoutManager == null)
                return;

            if (_cache.Words != null)
                RenderWords(e);

            base.OnPaint(e);
        }

        private void RenderWords(ElementPaintEventArgs e)
        {
            var foreColor = ForeColor.GetValueOrDefault((Color)SystemColors.ControlText);

            bool stop = false;

            foreach (var words in _cache.Words)
            {
                for (int i = 0; i < words.Length; i++)
                {
                    // No use rendering after the first clipped word.

                    if (words[i].Clip == WordClip.Clip)
                    {
                        stop = true;
                        break;
                    }

                    if (!(words[i].IsSpace && words[i].IsLineBreakAfter) && !words[i].IsNewline)
                        words[i].Render(e, words[i].Run.ForeColor.GetValueOrDefault(foreColor));

                    // The algorithm that determines the clip for the words
                    // may mark multiple words as trim. Because of this, we stop
                    // after the first trimmed word.

                    if (words[i].Clip != WordClip.None)
                    {
                        stop = true;
                        break;
                    }
                }

                if (stop)
                    break;
            }
        }

        internal void RegisterMouseOver(Run element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            _mouseOver[element] = true;
        }

        private void PrepareMouseLeave()
        {
            foreach (var item in new List<Run>(_mouseOver.Keys))
            {
                _mouseOver[item] = false;
            }
        }

        private void ProcessMouseLeave()
        {
            var toRemove = new List<Run>();

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

        private void BubbleEvent<T>(T e, Action<Run, T> runHandler, Action<T> baseHandler)
            where T : MouseEventArgs
        {
            var mouseLocation = e.GetPosition(this);

            if (_cache.Words != null)
            {
                foreach (var words in _cache.Words)
                {
                    for (int i = 0; i < words.Length; i++)
                    {
                        if (words[i].Bounds.Contains(mouseLocation))
                        {
                            runHandler(words[i].Run, e);
                            break;
                        }
                    }
                }
            }

            if (!e.IsBubblePrevented)
                baseHandler(e);
        }
    }
}
