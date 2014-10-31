using System;
using System.Collections.Generic;
using System.Text;

namespace GdiPresentation
{
    public static class ObjectUtil
    {
        public static bool Equals<T>(T a, object b)
            where T : class
        {
            if (ReferenceEquals(a, b))
                return true;

            var other = b as T;

            if (other == null)
                return false;

            return EqualsCore(a, other);
        }

        public static bool Equals<T>(T a, T b)
            where T : class
        {
            if (ReferenceEquals(a, b))
                return true;

            return EqualsCore(a, b);
        }

        private static bool EqualsCore<T>(T a, T b)
            where T : class
        {
            var equatable = a as IEquatable<T>;

            if (equatable != null)
                return EqualsCore(equatable, (IEquatable<T>)b);

            var genericComparable = a as IComparable<T>;

            if (genericComparable != null)
                return ComparableCore(genericComparable, b) == 0;

            var comparable = a as IComparable;

            if (comparable != null)
                return ComparableCore(comparable, b) == 0;

            throw new ArgumentException("Types are incompatible");
        }

        private static bool EqualsCore<T>(IEquatable<T> a, T b)
            where T : class
        {
            return a.Equals(b);
        }

        private static int ComparableCore<T>(IComparable<T> a, T b)
            where T : class
        {
            return a.CompareTo(b);
        }

        private static int ComparableCore(IComparable a, object b)
        {
            return a.CompareTo(b);
        }

        public static int CombineHashCodes(int hash1, int hash2)
        {
            unchecked
            {
                return (((hash1 << 5) + hash1) ^ hash2);
            }
        }

        public static int CombineHashCodes(int hash1, int hash2, int hash3)
        {
            return CombineHashCodes(CombineHashCodes(hash1, hash2), hash3);
        }

        public static int CombineHashCodes(int hash1, int hash2, int hash3, int hash4)
        {
            return CombineHashCodes(CombineHashCodes(hash1, hash2), CombineHashCodes(hash3, hash4));
        }

        public static int CombineHashCodes(int hash1, int hash2, int hash3, int hash4, int hash5)
        {
            return CombineHashCodes(CombineHashCodes(hash1, hash2, hash3, hash4), hash5);
        }

        public static int CombineHashCodes(int hash1, int hash2, int hash3, int hash4, int hash5, int hash6)
        {
            return CombineHashCodes(CombineHashCodes(hash1, hash2, hash3, hash4), CombineHashCodes(hash5, hash6));
        }

        public static int CombineHashCodes(int hash1, int hash2, int hash3, int hash4, int hash5, int hash6, int hash7)
        {
            return CombineHashCodes(CombineHashCodes(hash1, hash2, hash3, hash4), CombineHashCodes(hash5, hash6, hash7));
        }

        public static int CombineHashCodes(int hash1, int hash2, int hash3, int hash4, int hash5, int hash6, int hash7, int hash8)
        {
            return CombineHashCodes(CombineHashCodes(hash1, hash2, hash3, hash4), CombineHashCodes(hash5, hash6, hash7, hash8));
        }

        public static int CombineHashCodes(int hash1, int hash2, int hash3, int hash4, int hash5, int hash6, int hash7, int hash8, int hash9)
        {
            return CombineHashCodes(CombineHashCodes(hash1, hash2, hash3, hash4), CombineHashCodes(CombineHashCodes(hash5, hash6, hash7, hash8), hash9));
        }
    }
}
