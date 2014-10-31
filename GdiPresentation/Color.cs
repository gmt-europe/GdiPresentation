using System;
using System.Collections.Generic;
using System.Text;

namespace GdiPresentation
{
    public struct Color : IEquatable<Color>
    {
        public static readonly Color Transparent = (Color)System.Drawing.Color.Transparent;
        public static readonly Color AliceBlue = (Color)System.Drawing.Color.AliceBlue;
        public static readonly Color AntiqueWhite = (Color)System.Drawing.Color.AntiqueWhite;
        public static readonly Color Aqua = (Color)System.Drawing.Color.Aqua;
        public static readonly Color Aquamarine = (Color)System.Drawing.Color.Aquamarine;
        public static readonly Color Azure = (Color)System.Drawing.Color.Azure;
        public static readonly Color Beige = (Color)System.Drawing.Color.Beige;
        public static readonly Color Bisque = (Color)System.Drawing.Color.Bisque;
        public static readonly Color Black = (Color)System.Drawing.Color.Black;
        public static readonly Color BlanchedAlmond = (Color)System.Drawing.Color.BlanchedAlmond;
        public static readonly Color Blue = (Color)System.Drawing.Color.Blue;
        public static readonly Color BlueViolet = (Color)System.Drawing.Color.BlueViolet;
        public static readonly Color Brown = (Color)System.Drawing.Color.Brown;
        public static readonly Color BurlyWood = (Color)System.Drawing.Color.BurlyWood;
        public static readonly Color CadetBlue = (Color)System.Drawing.Color.CadetBlue;
        public static readonly Color Chartreuse = (Color)System.Drawing.Color.Chartreuse;
        public static readonly Color Chocolate = (Color)System.Drawing.Color.Chocolate;
        public static readonly Color Coral = (Color)System.Drawing.Color.Coral;
        public static readonly Color CornflowerBlue = (Color)System.Drawing.Color.CornflowerBlue;
        public static readonly Color Cornsilk = (Color)System.Drawing.Color.Cornsilk;
        public static readonly Color Crimson = (Color)System.Drawing.Color.Crimson;
        public static readonly Color Cyan = (Color)System.Drawing.Color.Cyan;
        public static readonly Color DarkBlue = (Color)System.Drawing.Color.DarkBlue;
        public static readonly Color DarkCyan = (Color)System.Drawing.Color.DarkCyan;
        public static readonly Color DarkGoldenrod = (Color)System.Drawing.Color.DarkGoldenrod;
        public static readonly Color DarkGray = (Color)System.Drawing.Color.DarkGray;
        public static readonly Color DarkGreen = (Color)System.Drawing.Color.DarkGreen;
        public static readonly Color DarkKhaki = (Color)System.Drawing.Color.DarkKhaki;
        public static readonly Color DarkMagenta = (Color)System.Drawing.Color.DarkMagenta;
        public static readonly Color DarkOliveGreen = (Color)System.Drawing.Color.DarkOliveGreen;
        public static readonly Color DarkOrange = (Color)System.Drawing.Color.DarkOrange;
        public static readonly Color DarkOrchid = (Color)System.Drawing.Color.DarkOrchid;
        public static readonly Color DarkRed = (Color)System.Drawing.Color.DarkRed;
        public static readonly Color DarkSalmon = (Color)System.Drawing.Color.DarkSalmon;
        public static readonly Color DarkSeaGreen = (Color)System.Drawing.Color.DarkSeaGreen;
        public static readonly Color DarkSlateBlue = (Color)System.Drawing.Color.DarkSlateBlue;
        public static readonly Color DarkSlateGray = (Color)System.Drawing.Color.DarkSlateGray;
        public static readonly Color DarkTurquoise = (Color)System.Drawing.Color.DarkTurquoise;
        public static readonly Color DarkViolet = (Color)System.Drawing.Color.DarkViolet;
        public static readonly Color DeepPink = (Color)System.Drawing.Color.DeepPink;
        public static readonly Color DeepSkyBlue = (Color)System.Drawing.Color.DeepSkyBlue;
        public static readonly Color DimGray = (Color)System.Drawing.Color.DimGray;
        public static readonly Color DodgerBlue = (Color)System.Drawing.Color.DodgerBlue;
        public static readonly Color Firebrick = (Color)System.Drawing.Color.Firebrick;
        public static readonly Color FloralWhite = (Color)System.Drawing.Color.FloralWhite;
        public static readonly Color ForestGreen = (Color)System.Drawing.Color.ForestGreen;
        public static readonly Color Fuchsia = (Color)System.Drawing.Color.Fuchsia;
        public static readonly Color Gainsboro = (Color)System.Drawing.Color.Gainsboro;
        public static readonly Color GhostWhite = (Color)System.Drawing.Color.GhostWhite;
        public static readonly Color Gold = (Color)System.Drawing.Color.Gold;
        public static readonly Color Goldenrod = (Color)System.Drawing.Color.Goldenrod;
        public static readonly Color Gray = (Color)System.Drawing.Color.Gray;
        public static readonly Color Green = (Color)System.Drawing.Color.Green;
        public static readonly Color GreenYellow = (Color)System.Drawing.Color.GreenYellow;
        public static readonly Color Honeydew = (Color)System.Drawing.Color.Honeydew;
        public static readonly Color HotPink = (Color)System.Drawing.Color.HotPink;
        public static readonly Color IndianRed = (Color)System.Drawing.Color.IndianRed;
        public static readonly Color Indigo = (Color)System.Drawing.Color.Indigo;
        public static readonly Color Ivory = (Color)System.Drawing.Color.Ivory;
        public static readonly Color Khaki = (Color)System.Drawing.Color.Khaki;
        public static readonly Color Lavender = (Color)System.Drawing.Color.Lavender;
        public static readonly Color LavenderBlush = (Color)System.Drawing.Color.LavenderBlush;
        public static readonly Color LawnGreen = (Color)System.Drawing.Color.LawnGreen;
        public static readonly Color LemonChiffon = (Color)System.Drawing.Color.LemonChiffon;
        public static readonly Color LightBlue = (Color)System.Drawing.Color.LightBlue;
        public static readonly Color LightCoral = (Color)System.Drawing.Color.LightCoral;
        public static readonly Color LightCyan = (Color)System.Drawing.Color.LightCyan;
        public static readonly Color LightGoldenrodYellow = (Color)System.Drawing.Color.LightGoldenrodYellow;
        public static readonly Color LightGreen = (Color)System.Drawing.Color.LightGreen;
        public static readonly Color LightGray = (Color)System.Drawing.Color.LightGray;
        public static readonly Color LightPink = (Color)System.Drawing.Color.LightPink;
        public static readonly Color LightSalmon = (Color)System.Drawing.Color.LightSalmon;
        public static readonly Color LightSeaGreen = (Color)System.Drawing.Color.LightSeaGreen;
        public static readonly Color LightSkyBlue = (Color)System.Drawing.Color.LightSkyBlue;
        public static readonly Color LightSlateGray = (Color)System.Drawing.Color.LightSlateGray;
        public static readonly Color LightSteelBlue = (Color)System.Drawing.Color.LightSteelBlue;
        public static readonly Color LightYellow = (Color)System.Drawing.Color.LightYellow;
        public static readonly Color Lime = (Color)System.Drawing.Color.Lime;
        public static readonly Color LimeGreen = (Color)System.Drawing.Color.LimeGreen;
        public static readonly Color Linen = (Color)System.Drawing.Color.Linen;
        public static readonly Color Magenta = (Color)System.Drawing.Color.Magenta;
        public static readonly Color Maroon = (Color)System.Drawing.Color.Maroon;
        public static readonly Color MediumAquamarine = (Color)System.Drawing.Color.MediumAquamarine;
        public static readonly Color MediumBlue = (Color)System.Drawing.Color.MediumBlue;
        public static readonly Color MediumOrchid = (Color)System.Drawing.Color.MediumOrchid;
        public static readonly Color MediumPurple = (Color)System.Drawing.Color.MediumPurple;
        public static readonly Color MediumSeaGreen = (Color)System.Drawing.Color.MediumSeaGreen;
        public static readonly Color MediumSlateBlue = (Color)System.Drawing.Color.MediumSlateBlue;
        public static readonly Color MediumSpringGreen = (Color)System.Drawing.Color.MediumSpringGreen;
        public static readonly Color MediumTurquoise = (Color)System.Drawing.Color.MediumTurquoise;
        public static readonly Color MediumVioletRed = (Color)System.Drawing.Color.MediumVioletRed;
        public static readonly Color MidnightBlue = (Color)System.Drawing.Color.MidnightBlue;
        public static readonly Color MintCream = (Color)System.Drawing.Color.MintCream;
        public static readonly Color MistyRose = (Color)System.Drawing.Color.MistyRose;
        public static readonly Color Moccasin = (Color)System.Drawing.Color.Moccasin;
        public static readonly Color NavajoWhite = (Color)System.Drawing.Color.NavajoWhite;
        public static readonly Color Navy = (Color)System.Drawing.Color.Navy;
        public static readonly Color OldLace = (Color)System.Drawing.Color.OldLace;
        public static readonly Color Olive = (Color)System.Drawing.Color.Olive;
        public static readonly Color OliveDrab = (Color)System.Drawing.Color.OliveDrab;
        public static readonly Color Orange = (Color)System.Drawing.Color.Orange;
        public static readonly Color OrangeRed = (Color)System.Drawing.Color.OrangeRed;
        public static readonly Color Orchid = (Color)System.Drawing.Color.Orchid;
        public static readonly Color PaleGoldenrod = (Color)System.Drawing.Color.PaleGoldenrod;
        public static readonly Color PaleGreen = (Color)System.Drawing.Color.PaleGreen;
        public static readonly Color PaleTurquoise = (Color)System.Drawing.Color.PaleTurquoise;
        public static readonly Color PaleVioletRed = (Color)System.Drawing.Color.PaleVioletRed;
        public static readonly Color PapayaWhip = (Color)System.Drawing.Color.PapayaWhip;
        public static readonly Color PeachPuff = (Color)System.Drawing.Color.PeachPuff;
        public static readonly Color Peru = (Color)System.Drawing.Color.Peru;
        public static readonly Color Pink = (Color)System.Drawing.Color.Pink;
        public static readonly Color Plum = (Color)System.Drawing.Color.Plum;
        public static readonly Color PowderBlue = (Color)System.Drawing.Color.PowderBlue;
        public static readonly Color Purple = (Color)System.Drawing.Color.Purple;
        public static readonly Color Red = (Color)System.Drawing.Color.Red;
        public static readonly Color RosyBrown = (Color)System.Drawing.Color.RosyBrown;
        public static readonly Color RoyalBlue = (Color)System.Drawing.Color.RoyalBlue;
        public static readonly Color SaddleBrown = (Color)System.Drawing.Color.SaddleBrown;
        public static readonly Color Salmon = (Color)System.Drawing.Color.Salmon;
        public static readonly Color SandyBrown = (Color)System.Drawing.Color.SandyBrown;
        public static readonly Color SeaGreen = (Color)System.Drawing.Color.SeaGreen;
        public static readonly Color SeaShell = (Color)System.Drawing.Color.SeaShell;
        public static readonly Color Sienna = (Color)System.Drawing.Color.Sienna;
        public static readonly Color Silver = (Color)System.Drawing.Color.Silver;
        public static readonly Color SkyBlue = (Color)System.Drawing.Color.SkyBlue;
        public static readonly Color SlateBlue = (Color)System.Drawing.Color.SlateBlue;
        public static readonly Color SlateGray = (Color)System.Drawing.Color.SlateGray;
        public static readonly Color Snow = (Color)System.Drawing.Color.Snow;
        public static readonly Color SpringGreen = (Color)System.Drawing.Color.SpringGreen;
        public static readonly Color SteelBlue = (Color)System.Drawing.Color.SteelBlue;
        public static readonly Color Tan = (Color)System.Drawing.Color.Tan;
        public static readonly Color Teal = (Color)System.Drawing.Color.Teal;
        public static readonly Color Thistle = (Color)System.Drawing.Color.Thistle;
        public static readonly Color Tomato = (Color)System.Drawing.Color.Tomato;
        public static readonly Color Turquoise = (Color)System.Drawing.Color.Turquoise;
        public static readonly Color Violet = (Color)System.Drawing.Color.Violet;
        public static readonly Color Wheat = (Color)System.Drawing.Color.Wheat;
        public static readonly Color White = (Color)System.Drawing.Color.White;
        public static readonly Color WhiteSmoke = (Color)System.Drawing.Color.WhiteSmoke;
        public static readonly Color Yellow = (Color)System.Drawing.Color.Yellow;
        public static readonly Color YellowGreen = (Color)System.Drawing.Color.YellowGreen;

        private readonly int _a;
        private readonly int _r;
        private readonly int _g;
        private readonly int _b;

        public int A
        {
            get { return _a; }
        }

        public int R
        {
            get { return _r; }
        }

        public int G
        {
            get { return _g; }
        }

        public int B
        {
            get { return _b; }
        }

        public Color(int r, int g, int b)
            : this(255, r, g, b)
        {
        }

        public Color(int a, int r, int g, int b)
            : this()
        {
            _a = a;
            _r = r;
            _g = g;
            _b = b;
        }

        public Color(int a, Color baseColor)
            : this(a, baseColor.R, baseColor.G, baseColor.B)
        {
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Color))
                return false;

            return Equals((Color)obj);
        }

        public bool Equals(Color other)
        {
            return
                _a == other._a &&
                _r == other._r &&
                _g == other._g &
                _b == other._b;
        }

        public static bool operator ==(Color a, Color b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Color a, Color b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return ObjectUtil.CombineHashCodes(
                _a.GetHashCode(), _r.GetHashCode(),
                _g.GetHashCode(), _b.GetHashCode()
            );
        }

        public static explicit operator Color(System.Drawing.Color other)
        {
            return new Color(other.A, other.R, other.G, other.B);
        }

        public static explicit operator System.Drawing.Color(Color other)
        {
            if (other.A == 0)
                return System.Drawing.Color.Transparent;
            else
                return System.Drawing.Color.FromArgb(other.A, other.R, other.G, other.B);
        }

        public static implicit operator HslColor(Color other)
        {
            return HslColor.FromColor((System.Drawing.Color)other);
        }

        public static implicit operator Color(HslColor other)
        {
            return (Color)other.ToColor();
        }

        public override string ToString()
        {
            return String.Format("A: {0}, R: {1}, G: {2}, B: {3}", _a, _r, _g, _b);
        }
    }
}
