using System;
using System.Collections.Generic;
using System.Text;

namespace GdiPresentation
{
    internal static class IntUtil
    {
        private const int Cutoff = int.MaxValue / 2;

        public static bool IsInfinite(int value)
        {
            return IsPositiveInfinite(value) || IsNegativeInfinite(value);
        }

        public static bool IsPositiveInfinite(int value)
        {
            return value > Cutoff;
        }

        public static bool IsNegativeInfinite(int value)
        {
            return value < -Cutoff;
        }
    }
}
