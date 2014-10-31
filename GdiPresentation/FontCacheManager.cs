using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GdiPresentation
{
    internal class FontCacheManager
    {
        [ThreadStatic]
        private static FontCacheManager _current;

        private readonly Dictionary<Key, Font> _cache = new Dictionary<Key, Font>();
        private readonly Dictionary<Font, Size> _paddingCache = new Dictionary<Font, Size>();

        public static FontCacheManager Current
        {
            get
            {
                if (_current == null)
                    _current = new FontCacheManager();

                return _current;
            }
        }

        public Font GetFont(string name, float size, FontStyle style)
        {
            var key = new Key(name, size, style);
            Font font;

            if (!_cache.TryGetValue(key, out font))
            {
                font = new Font(name, size, (System.Drawing.FontStyle)style);
                _cache[key] = font;
            }

            return font;
        }

        public Size GetPadding(Font font)
        {
            if (font == null)
                throw new ArgumentNullException("font");

            Size padding;

            if (!_paddingCache.TryGetValue(font, out padding))
            {
                var largeSize = TextRenderer.MeasureText(
                    "  ",
                    font,
                    new System.Drawing.Size(int.MaxValue, int.MaxValue),
                    WordCache.FormatFlags
                );
                var smallSize = TextRenderer.MeasureText(
                    " ",
                    font,
                    new System.Drawing.Size(int.MaxValue, int.MaxValue),
                    WordCache.FormatFlags
                );

                padding = new Size(
                    smallSize.Width - (largeSize.Width - smallSize.Width),
                    0
                );

                _paddingCache[font] = padding;
            }

            return padding;
        }

        private struct Key : IEquatable<Key>
        {
            private readonly string _name;
            private readonly float _size;
            private readonly FontStyle _style;

            public Key(string name, float size, FontStyle style)
            {
                if (name == null)
                    throw new ArgumentNullException("name");

                _name = name;
                _size = size;
                _style = style;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is Key))
                    return false;

                return Equals((Key)obj);
            }

            public bool Equals(Key other)
            {
                return
                    _name == other._name &&
                    _size == other._size &&
                    _style == other._style;
            }

            public override int GetHashCode()
            {
                return ObjectUtil.CombineHashCodes(
                    _name.GetHashCode(),
                    _size.GetHashCode(),
                    _style.GetHashCode()
                );
            }
        }
    }
}
