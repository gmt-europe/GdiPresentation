using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GdiPresentation
{
    internal struct WordCache
    {
        public const TextFormatFlags FormatFlags = TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine | TextFormatFlags.PreserveGraphicsTranslateTransform | TextFormatFlags.PreserveGraphicsClipping;

        public readonly Run Run;
        public readonly string Text;
        public readonly bool IsSpace;
        public readonly bool IsNewline;
        public readonly Font Font;
        public readonly Size Size;
        public Point Location;
        public bool IsLineBreakAfter;
        public WordClip Clip;

        public Rect Bounds
        {
            get { return new Rect(Location, Size); }
        }

        public WordCache(Run run, string text, Font font)
        {
            if (run == null)
                throw new ArgumentNullException("run");
            if (text == null)
                throw new ArgumentNullException("text");

            Run = run;
            Text = text;
            Font = font;

            Location = new Point();
            IsLineBreakAfter = false;
            Clip = WordClip.None;

            IsNewline = text == "\n";

            if (IsNewline || Text.Length == 0)
            {
                IsSpace = false;
                Size = new Size();
            }
            else
            {
                var padding = FontCacheManager.Current.GetPadding(font);

                Size = (Size)TextRenderer.MeasureText(
                    Text,
                    font,
                    new System.Drawing.Size(int.MaxValue, int.MaxValue),
                    FormatFlags
                );

                Size = new Size(
                    Size.Width - padding.Width,
                    Size.Height - padding.Height
                );

                IsSpace = true;

                foreach (char c in text)
                {
                    if (!Char.IsWhiteSpace(c))
                    {
                        IsSpace = false;
                        break;
                    }
                }
            }
        }

        public void Render(ElementPaintEventArgs e, Color foreColor)
        {
            Debug.Assert(Clip != WordClip.Clip);

            string text = Text;
            var format = FormatFlags;

            var bounds = new Rectangle(
                e.Bounds.X + Location.X,
                e.Bounds.Y + Location.Y,
                e.Bounds.Width - Location.X,
                Math.Min(Size.Height, e.Bounds.Height - Location.Y)
            );

            switch (Clip)
            {
                case WordClip.AppendEllipsis:
                    text += "...";
                    break;

                case WordClip.ReplaceWithEllipsis:
                    text = "...";
                    break;

                case WordClip.Trim:
                    // We fake the hell out of this to make sure TextRenderer
                    // draws the ellipsis. We append the ellipsis here because
                    // on word breaks, the text may not be long enough to
                    // force the ellipsis. Adding it ourselves ensures that the
                    // ellipsis is drawn. Otherwise, the ellipsis is ellipsis-d.

                    text += "...";
                    format |= TextFormatFlags.EndEllipsis;
                    break;
            }

            TextRenderer.DrawText(
                e.Graphics,
                text,
                Font,
                bounds,
                (System.Drawing.Color)foreColor,
                System.Drawing.Color.Transparent,
                format
            );
        }
    }
}
