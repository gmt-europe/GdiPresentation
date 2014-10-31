using System;
using System.Collections.Generic;
using System.Text;

namespace GdiPresentation
{
    public struct Vector : IEquatable<Vector>
    {
        public static Vector Empty
        {
            get { return new Vector(); }
        }

        private int _x;
        private int _y;

        public int X
        {
            get { return _x; }
            set { _x = value; }
        }

        public int Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public bool IsEmpty
        {
            get { return this == Empty; }
        }

        public Vector(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector))
                return false;

            return Equals((Vector)obj);
        }

        public bool Equals(Vector other)
        {
            return
                _x == other._x &&
                _y == other._y;
        }

        public override int GetHashCode()
        {
            return ObjectUtil.CombineHashCodes(
                _x.GetHashCode(),
                _y.GetHashCode()
            );
        }

        public static bool operator ==(Vector a, Vector b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Vector a, Vector b)
        {
            return !(a == b);
        }

        public static explicit operator System.Drawing.Point(Vector other)
        {
            return new System.Drawing.Point(
                other.X,
                other.Y
            );
        }

        public static explicit operator Vector(System.Drawing.Point other)
        {
            return new Vector(
                other.X,
                other.Y
            );
        }

        public static implicit operator Point(Vector other)
        {
            return new Point(other.X, other.Y);
        }

        public static implicit operator Vector(Point other)
        {
            return new Vector(other.X, other.Y);
        }

        public override string ToString()
        {
            return String.Format("X: {0}, Y: {1}", _x, _y);
        }
    }
}
