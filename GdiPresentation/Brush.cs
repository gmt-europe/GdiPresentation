using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GdiPresentation
{
    public abstract class Brush
    {
        public static readonly Brush Transparent = new SolidBrush(Color.Transparent);
        public static readonly Brush AliceBlue = new SolidBrush(Color.AliceBlue);
        public static readonly Brush AntiqueWhite = new SolidBrush(Color.AntiqueWhite);
        public static readonly Brush Aqua = new SolidBrush(Color.Aqua);
        public static readonly Brush Aquamarine = new SolidBrush(Color.Aquamarine);
        public static readonly Brush Azure = new SolidBrush(Color.Azure);
        public static readonly Brush Beige = new SolidBrush(Color.Beige);
        public static readonly Brush Bisque = new SolidBrush(Color.Bisque);
        public static readonly Brush Black = new SolidBrush(Color.Black);
        public static readonly Brush BlanchedAlmond = new SolidBrush(Color.BlanchedAlmond);
        public static readonly Brush Blue = new SolidBrush(Color.Blue);
        public static readonly Brush BlueViolet = new SolidBrush(Color.BlueViolet);
        public static readonly Brush Brown = new SolidBrush(Color.Brown);
        public static readonly Brush BurlyWood = new SolidBrush(Color.BurlyWood);
        public static readonly Brush CadetBlue = new SolidBrush(Color.CadetBlue);
        public static readonly Brush Chartreuse = new SolidBrush(Color.Chartreuse);
        public static readonly Brush Chocolate = new SolidBrush(Color.Chocolate);
        public static readonly Brush Coral = new SolidBrush(Color.Coral);
        public static readonly Brush CornflowerBlue = new SolidBrush(Color.CornflowerBlue);
        public static readonly Brush Cornsilk = new SolidBrush(Color.Cornsilk);
        public static readonly Brush Crimson = new SolidBrush(Color.Crimson);
        public static readonly Brush Cyan = new SolidBrush(Color.Cyan);
        public static readonly Brush DarkBlue = new SolidBrush(Color.DarkBlue);
        public static readonly Brush DarkCyan = new SolidBrush(Color.DarkCyan);
        public static readonly Brush DarkGoldenrod = new SolidBrush(Color.DarkGoldenrod);
        public static readonly Brush DarkGray = new SolidBrush(Color.DarkGray);
        public static readonly Brush DarkGreen = new SolidBrush(Color.DarkGreen);
        public static readonly Brush DarkKhaki = new SolidBrush(Color.DarkKhaki);
        public static readonly Brush DarkMagenta = new SolidBrush(Color.DarkMagenta);
        public static readonly Brush DarkOliveGreen = new SolidBrush(Color.DarkOliveGreen);
        public static readonly Brush DarkOrange = new SolidBrush(Color.DarkOrange);
        public static readonly Brush DarkOrchid = new SolidBrush(Color.DarkOrchid);
        public static readonly Brush DarkRed = new SolidBrush(Color.DarkRed);
        public static readonly Brush DarkSalmon = new SolidBrush(Color.DarkSalmon);
        public static readonly Brush DarkSeaGreen = new SolidBrush(Color.DarkSeaGreen);
        public static readonly Brush DarkSlateBlue = new SolidBrush(Color.DarkSlateBlue);
        public static readonly Brush DarkSlateGray = new SolidBrush(Color.DarkSlateGray);
        public static readonly Brush DarkTurquoise = new SolidBrush(Color.DarkTurquoise);
        public static readonly Brush DarkViolet = new SolidBrush(Color.DarkViolet);
        public static readonly Brush DeepPink = new SolidBrush(Color.DeepPink);
        public static readonly Brush DeepSkyBlue = new SolidBrush(Color.DeepSkyBlue);
        public static readonly Brush DimGray = new SolidBrush(Color.DimGray);
        public static readonly Brush DodgerBlue = new SolidBrush(Color.DodgerBlue);
        public static readonly Brush Firebrick = new SolidBrush(Color.Firebrick);
        public static readonly Brush FloralWhite = new SolidBrush(Color.FloralWhite);
        public static readonly Brush ForestGreen = new SolidBrush(Color.ForestGreen);
        public static readonly Brush Fuchsia = new SolidBrush(Color.Fuchsia);
        public static readonly Brush Gainsboro = new SolidBrush(Color.Gainsboro);
        public static readonly Brush GhostWhite = new SolidBrush(Color.GhostWhite);
        public static readonly Brush Gold = new SolidBrush(Color.Gold);
        public static readonly Brush Goldenrod = new SolidBrush(Color.Goldenrod);
        public static readonly Brush Gray = new SolidBrush(Color.Gray);
        public static readonly Brush Green = new SolidBrush(Color.Green);
        public static readonly Brush GreenYellow = new SolidBrush(Color.GreenYellow);
        public static readonly Brush Honeydew = new SolidBrush(Color.Honeydew);
        public static readonly Brush HotPink = new SolidBrush(Color.HotPink);
        public static readonly Brush IndianRed = new SolidBrush(Color.IndianRed);
        public static readonly Brush Indigo = new SolidBrush(Color.Indigo);
        public static readonly Brush Ivory = new SolidBrush(Color.Ivory);
        public static readonly Brush Khaki = new SolidBrush(Color.Khaki);
        public static readonly Brush Lavender = new SolidBrush(Color.Lavender);
        public static readonly Brush LavenderBlush = new SolidBrush(Color.LavenderBlush);
        public static readonly Brush LawnGreen = new SolidBrush(Color.LawnGreen);
        public static readonly Brush LemonChiffon = new SolidBrush(Color.LemonChiffon);
        public static readonly Brush LightBlue = new SolidBrush(Color.LightBlue);
        public static readonly Brush LightCoral = new SolidBrush(Color.LightCoral);
        public static readonly Brush LightCyan = new SolidBrush(Color.LightCyan);
        public static readonly Brush LightGoldenrodYellow = new SolidBrush(Color.LightGoldenrodYellow);
        public static readonly Brush LightGreen = new SolidBrush(Color.LightGreen);
        public static readonly Brush LightGray = new SolidBrush(Color.LightGray);
        public static readonly Brush LightPink = new SolidBrush(Color.LightPink);
        public static readonly Brush LightSalmon = new SolidBrush(Color.LightSalmon);
        public static readonly Brush LightSeaGreen = new SolidBrush(Color.LightSeaGreen);
        public static readonly Brush LightSkyBlue = new SolidBrush(Color.LightSkyBlue);
        public static readonly Brush LightSlateGray = new SolidBrush(Color.LightSlateGray);
        public static readonly Brush LightSteelBlue = new SolidBrush(Color.LightSteelBlue);
        public static readonly Brush LightYellow = new SolidBrush(Color.LightYellow);
        public static readonly Brush Lime = new SolidBrush(Color.Lime);
        public static readonly Brush LimeGreen = new SolidBrush(Color.LimeGreen);
        public static readonly Brush Linen = new SolidBrush(Color.Linen);
        public static readonly Brush Magenta = new SolidBrush(Color.Magenta);
        public static readonly Brush Maroon = new SolidBrush(Color.Maroon);
        public static readonly Brush MediumAquamarine = new SolidBrush(Color.MediumAquamarine);
        public static readonly Brush MediumBlue = new SolidBrush(Color.MediumBlue);
        public static readonly Brush MediumOrchid = new SolidBrush(Color.MediumOrchid);
        public static readonly Brush MediumPurple = new SolidBrush(Color.MediumPurple);
        public static readonly Brush MediumSeaGreen = new SolidBrush(Color.MediumSeaGreen);
        public static readonly Brush MediumSlateBlue = new SolidBrush(Color.MediumSlateBlue);
        public static readonly Brush MediumSpringGreen = new SolidBrush(Color.MediumSpringGreen);
        public static readonly Brush MediumTurquoise = new SolidBrush(Color.MediumTurquoise);
        public static readonly Brush MediumVioletRed = new SolidBrush(Color.MediumVioletRed);
        public static readonly Brush MidnightBlue = new SolidBrush(Color.MidnightBlue);
        public static readonly Brush MintCream = new SolidBrush(Color.MintCream);
        public static readonly Brush MistyRose = new SolidBrush(Color.MistyRose);
        public static readonly Brush Moccasin = new SolidBrush(Color.Moccasin);
        public static readonly Brush NavajoWhite = new SolidBrush(Color.NavajoWhite);
        public static readonly Brush Navy = new SolidBrush(Color.Navy);
        public static readonly Brush OldLace = new SolidBrush(Color.OldLace);
        public static readonly Brush Olive = new SolidBrush(Color.Olive);
        public static readonly Brush OliveDrab = new SolidBrush(Color.OliveDrab);
        public static readonly Brush Orange = new SolidBrush(Color.Orange);
        public static readonly Brush OrangeRed = new SolidBrush(Color.OrangeRed);
        public static readonly Brush Orchid = new SolidBrush(Color.Orchid);
        public static readonly Brush PaleGoldenrod = new SolidBrush(Color.PaleGoldenrod);
        public static readonly Brush PaleGreen = new SolidBrush(Color.PaleGreen);
        public static readonly Brush PaleTurquoise = new SolidBrush(Color.PaleTurquoise);
        public static readonly Brush PaleVioletRed = new SolidBrush(Color.PaleVioletRed);
        public static readonly Brush PapayaWhip = new SolidBrush(Color.PapayaWhip);
        public static readonly Brush PeachPuff = new SolidBrush(Color.PeachPuff);
        public static readonly Brush Peru = new SolidBrush(Color.Peru);
        public static readonly Brush Pink = new SolidBrush(Color.Pink);
        public static readonly Brush Plum = new SolidBrush(Color.Plum);
        public static readonly Brush PowderBlue = new SolidBrush(Color.PowderBlue);
        public static readonly Brush Purple = new SolidBrush(Color.Purple);
        public static readonly Brush Red = new SolidBrush(Color.Red);
        public static readonly Brush RosyBrown = new SolidBrush(Color.RosyBrown);
        public static readonly Brush RoyalBlue = new SolidBrush(Color.RoyalBlue);
        public static readonly Brush SaddleBrown = new SolidBrush(Color.SaddleBrown);
        public static readonly Brush Salmon = new SolidBrush(Color.Salmon);
        public static readonly Brush SandyBrown = new SolidBrush(Color.SandyBrown);
        public static readonly Brush SeaGreen = new SolidBrush(Color.SeaGreen);
        public static readonly Brush SeaShell = new SolidBrush(Color.SeaShell);
        public static readonly Brush Sienna = new SolidBrush(Color.Sienna);
        public static readonly Brush Silver = new SolidBrush(Color.Silver);
        public static readonly Brush SkyBlue = new SolidBrush(Color.SkyBlue);
        public static readonly Brush SlateBlue = new SolidBrush(Color.SlateBlue);
        public static readonly Brush SlateGray = new SolidBrush(Color.SlateGray);
        public static readonly Brush Snow = new SolidBrush(Color.Snow);
        public static readonly Brush SpringGreen = new SolidBrush(Color.SpringGreen);
        public static readonly Brush SteelBlue = new SolidBrush(Color.SteelBlue);
        public static readonly Brush Tan = new SolidBrush(Color.Tan);
        public static readonly Brush Teal = new SolidBrush(Color.Teal);
        public static readonly Brush Thistle = new SolidBrush(Color.Thistle);
        public static readonly Brush Tomato = new SolidBrush(Color.Tomato);
        public static readonly Brush Turquoise = new SolidBrush(Color.Turquoise);
        public static readonly Brush Violet = new SolidBrush(Color.Violet);
        public static readonly Brush Wheat = new SolidBrush(Color.Wheat);
        public static readonly Brush White = new SolidBrush(Color.White);
        public static readonly Brush WhiteSmoke = new SolidBrush(Color.WhiteSmoke);
        public static readonly Brush Yellow = new SolidBrush(Color.Yellow);
        public static readonly Brush YellowGreen = new SolidBrush(Color.YellowGreen);

        internal abstract System.Drawing.Brush CreateBrush(Rect bounds);
    }

    public class SolidBrush : Brush
    {
        public Color Color { get; private set; }

        public SolidBrush(Color color)
        {
            Color = color;
        }

        internal override System.Drawing.Brush CreateBrush(Rect bounds)
        {
            return new System.Drawing.SolidBrush((System.Drawing.Color)Color);
        }
    }

    public class GradientBrush : Brush
    {
        private readonly bool _haveAngle;

        public Color Color1 { get; private set; }

        public Color Color2 { get; private set; }

        public LinearGradientMode Mode { get; private set; }

        public float Angle { get; private set; }

        public GradientBrush(Color color1, Color color2, LinearGradientMode mode)
            : this(color1, color2, mode, 0)
        {
        }

        public GradientBrush(Color color1, Color color2, float angle)
            : this(color1, color2, LinearGradientMode.Horizontal, angle)
        {
            _haveAngle = true;
        }

        private GradientBrush(Color color1, Color color2, LinearGradientMode mode, float angle)
        {
            Color1 = color1;
            Color2 = color2;
            Mode = mode;
            Angle = angle;
        }

        internal override System.Drawing.Brush CreateBrush(Rect bounds)
        {
            if (_haveAngle)
            {
                return new System.Drawing.Drawing2D.LinearGradientBrush(
                    (Rectangle)bounds,
                    (System.Drawing.Color)Color1,
                    (System.Drawing.Color)Color2,
                    Angle
                );
            }
            else
            {
                return new System.Drawing.Drawing2D.LinearGradientBrush(
                    (Rectangle)bounds,
                    (System.Drawing.Color)Color1,
                    (System.Drawing.Color)Color2,
                    (System.Drawing.Drawing2D.LinearGradientMode)Mode
                );
            }
        }
    }
}
