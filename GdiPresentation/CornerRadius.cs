using System;
using System.Collections.Generic;
using System.Text;

namespace GdiPresentation
{
    public struct CornerRadius : IEquatable<CornerRadius>
    {
        public static readonly CornerRadius Empty = new CornerRadius();

        private readonly int _topLeft;
        private readonly int _topRight;
        private readonly int _bottomLeft;
        private readonly int _bottomRight;
        private readonly bool _isNotEmpty;

        public int TopLeft
        {
            get { return _topLeft; }
        }

        public int TopRight
        {
            get { return _topRight; }
        }

        public int BottomLeft
        {
            get { return _bottomLeft; }
        }

        public int BottomRight
        {
            get { return _bottomRight; }
        }

        public bool IsEmpty
        {
            get { return !_isNotEmpty; }
        }

        public CornerRadius(int topLeft, int topRight, int bottomLeft, int bottomRight)
        {
            if (topLeft < 0)
                throw new ArgumentOutOfRangeException("topLeft");
            if (topRight < 0)
                throw new ArgumentOutOfRangeException("topRight");
            if (bottomLeft < 0)
                throw new ArgumentOutOfRangeException("bottomLeft");
            if (bottomRight < 0)
                throw new ArgumentOutOfRangeException("bottomRight");

            _topLeft = topLeft;
            _topRight = topRight;
            _bottomLeft = bottomLeft;
            _bottomRight = bottomRight;
            _isNotEmpty = (topLeft + topRight + bottomLeft + bottomRight) != 0;
        }

        public CornerRadius(int uniformRadius)
            : this(uniformRadius, uniformRadius, uniformRadius, uniformRadius)
        {
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CornerRadius))
                return false;

            return Equals((CornerRadius)obj);
        }

        public bool Equals(CornerRadius other)
        {
            return
                _topLeft == other._topLeft &&
                _topRight == other._topRight &&
                _bottomLeft == other._bottomLeft &&
                _bottomRight == other._bottomRight;
        }

        public static bool operator ==(CornerRadius a, CornerRadius b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(CornerRadius a, CornerRadius b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return ObjectUtil.CombineHashCodes(
                _topLeft.GetHashCode(),
                _topRight.GetHashCode(),
                _bottomLeft.GetHashCode(),
                _bottomRight.GetHashCode()
            );
        }
    }
}
