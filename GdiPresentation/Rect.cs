using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GdiPresentation
{
    public struct Rect : IEquatable<Rect>
    {
        public static Rect Empty
        {
            get { return new Rect(); }
        }

        private int _left;
        private int _top;
        private int _width;
        private int _height;

        public int X
        {
            get { return _left; }
            set { _left = value; }
        }

        public int Y
        {
            get { return _top; }
            set { _top = value; }
        }

        public int Left
        {
            get { return _left; }
            set { _left = value; }
        }

        public int Top
        {
            get { return _top; }
            set { _top = value; }
        }

        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        public int Right
        {
            get { return _left + _width; }
        }

        public int Bottom
        {
            get { return _top + _height; }
        }

        public Size Size
        {
            get { return new Size(_width, _height); }
        }

        public Point Location
        {
            get { return new Point(_left, _top); }
        }

        public bool IsEmpty
        {
            get { return this == Empty; }
        }

        public Rect(int left, int top, int width, int height)
        {
            _left = left;
            _top = top;
            _width = width;
            _height = height;
        }

        public Rect(Point location, Size size)
            : this(location.X, location.Y, size.Width, size.Height)
        {
        }

        public Rect(Vector location, Size size)
            : this(location.X, location.Y, size.Width, size.Height)
        {
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Rect))
                return false;

            return Equals((Rect)obj);
        }

        public bool Equals(Rect other)
        {
            return
                _left == other._left &&
                _top == other._top &&
                _width == other._width &&
                _height == other._height;
        }

        public override int GetHashCode()
        {
            return ObjectUtil.CombineHashCodes(
                _left.GetHashCode(),
                _top.GetHashCode(),
                _width.GetHashCode(),
                _height.GetHashCode()
            );
        }

        public static bool operator ==(Rect a, Rect b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Rect a, Rect b)
        {
            return !(a == b);
        }

        public bool Contains(Point location)
        {
            return
                _left <= location.X &&
                _top <= location.Y &&
                Right >= location.X &&
                Bottom >= location.Y;
                
        }

        public static explicit operator Rect(Rectangle other)
        {
            return new Rect(
                other.Left,
                other.Top,
                other.Width,
                other.Height
            );
        }

        public static explicit operator Rectangle(Rect other)
        {
            return new Rectangle(
                other.Left,
                other.Top,
                other.Width,
                other.Height
            );
        }

        public bool IntersectsWith(Rect other)
        {
            return
                other.Left < Left + Width &&
                Left < other.Left + other.Width &&
                other.Top < Top + Height &&
                Top < other.Top + other.Height;
        }

        public void Inflate(int horizontal, int vertical)
        {
            _left -= horizontal;
            _width += horizontal * 2;
            _top -= vertical;
            _height += vertical * 2;
        }

        public override string ToString()
        {
            return String.Format(
                "X: {0}, Y: {1}, Width: {2}, Height: {3}",
                _left, _top, _width, _height
            );
        }
    }
}
