using System;
using System.Collections.Generic;
using System.Text;

namespace GdiPresentation
{
    public struct GridLength : IEquatable<GridLength>
    {
        public static readonly GridLength Auto = new GridLength(GridUnitType.Auto);

        private readonly GridUnitType _gridUnitType;
        private readonly int _value;

        public GridLength(GridUnitType gridUnitType)
            : this(gridUnitType, 0)
        {
        }

        public GridLength(GridUnitType gridUnitType, int value)
        {
            if (gridUnitType == GridUnitType.Star && value < 1)
                throw new ArgumentException("Star grid unit must be at least one");

            _gridUnitType = gridUnitType;
            _value = value;
        }

        public GridUnitType GridUnitType
        {
            get { return _gridUnitType; }
        }

        public int Value
        {
            get { return _value; }
        }

        public bool IsAuto
        {
            get { return _gridUnitType == GridUnitType.Auto; }
        }

        public bool IsStar
        {
            get { return _gridUnitType == GridUnitType.Star; }
        }

        public bool IsPixel
        {
            get { return _gridUnitType == GridUnitType.Pixel; }
        }

        public static bool operator ==(GridLength a, GridLength b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(GridLength a, GridLength b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is GridLength))
                return false;

            return Equals((GridLength)obj);
        }

        public bool Equals(GridLength other)
        {
            return
                _gridUnitType == other._gridUnitType &&
                _value == other._value;
        }

        public override int GetHashCode()
        {
            return ObjectUtil.CombineHashCodes(
                _gridUnitType.GetHashCode(),
                _value.GetHashCode()
            );
        }
    }
}
