using System;
using System.Collections.Generic;
using System.Text;

namespace GdiPresentation
{
    public struct Point : IEquatable<Point>
    {
        public static Point Empty
        {
            get { return new Point(); }
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

        public Point(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Point))
                return false;

            return Equals((Point)obj);
        }

        public bool Equals(Point other)
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

        public static bool operator ==(Point a, Point b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Point a, Point b)
        {
            return !(a == b);
        }

        public static explicit operator System.Drawing.Point(Point other)
        {
            return new System.Drawing.Point(
                other.X,
                other.Y
            );
        }

        public static explicit operator Point(System.Drawing.Point other)
        {
            return new Point(
                other.X,
                other.Y
            );
        }

        public static Point operator +(Point a, Point b)
        {
            return new Point(
                a.X + b.X,
                a.Y + b.Y
            );
        }

        public static Point operator -(Point a, Point b)
        {
            return new Point(
                a.X - b.X,
                a.Y - b.Y
            );
        }

        public override string ToString()
        {
            return String.Format("X: {0}, Y: {1}", _x, _y);
        }
    }
}
