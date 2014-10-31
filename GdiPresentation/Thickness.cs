using System;
using System.Collections.Generic;
using System.Text;

namespace GdiPresentation
{
    public struct Thickness : IEquatable<Thickness>
    {
        public static readonly Thickness Empty = new Thickness();

        private readonly int _left;
        private readonly int _top;
        private readonly int _right;
        private readonly int _bottom;
        private readonly bool _isNotEmpty;

        public int Left
        {
            get { return _left; }
        }

        public int Top
        {
            get { return _top; }
        }

        public int Right
        {
            get { return _right; }
        }

        public int Bottom
        {
            get { return _bottom; }
        }

        public int Horizontal
        {
            get { return _left + _right; }
        }

        public int Vertical
        {
            get { return _top + _bottom; }
        }

        public bool IsEmpty
        {
            get { return !_isNotEmpty; }
        }

        public Thickness(int left, int top, int right, int bottom)
        {
            _left = left;
            _top = top;
            _right = right;
            _bottom = bottom;

            _isNotEmpty = !(left == 0 && top == 0 && right == 0 && bottom == 0);
        }

        public Thickness(int all)
            : this(all, all, all, all)
        {
        }

        public Thickness(int leftRight, int topBottom)
            : this(leftRight, topBottom, leftRight, topBottom)
        {
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Thickness))
                return false;

            return Equals((Thickness)obj);
        }

        public bool Equals(Thickness other)
        {
            return
                _left == other._left &&
                _top == other._top &&
                _right == other._right &&
                _bottom == other._bottom;
        }

        public static bool operator ==(Thickness a, Thickness b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Thickness a, Thickness b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return ObjectUtil.CombineHashCodes(
                _left.GetHashCode(),
                _top.GetHashCode(),
                _right.GetHashCode(),
                _bottom.GetHashCode()
            );
        }
    }
}
