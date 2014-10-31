using System;
using System.Collections.Generic;
using System.Text;

namespace GdiPresentation
{
    public struct Size : IEquatable<Size>
    {
        public static Size Empty
        {
            get { return new Size(); }
        }

        private int _width;
        private int _height;

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

        public bool IsEmpty
        {
            get { return this == Empty; }
        }

        public Size(int width, int height)
        {
            _width = width;
            _height = height;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Size))
                return false;

            return Equals((Size)obj);
        }

        public bool Equals(Size other)
        {
            return
                _width == other._width &&
                _height == other._height;
        }

        public override int GetHashCode()
        {
            return ObjectUtil.CombineHashCodes(
                _width.GetHashCode(),
                _height.GetHashCode()
            );
        }

        public static bool operator ==(Size a, Size b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Size a, Size b)
        {
            return !(a == b);
        }

        public static explicit operator Size(System.Drawing.Size other)
        {
            return new Size(
                other.Width,
                other.Height
            );
        }

        public static explicit operator System.Drawing.Size(Size other)
        {
            return new System.Drawing.Size(
                other.Width,
                other.Height
            );
        }

        public override string ToString()
        {
            return String.Format("Width: {0}, Height: {1}", _width, _height);
        }
    }
}
