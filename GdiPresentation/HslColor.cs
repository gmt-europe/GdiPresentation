// Taken from http://richnewman.wordpress.com/about/code-listings-and-diagrams/hslcolor-class/
// and cleaned up significantly.

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace GdiPresentation
{
    public struct HslColor
    {
        private readonly int _alpha;
        private readonly double _hue;
        private readonly double _saturation;
        private readonly double _luminosity;

        public int Alpha
        {
            get { return _alpha; }
        }

        public double Hue
        {
            get { return _hue; }
        }

        public double Saturation
        {
            get { return _saturation; }
        }

        public double Luminosity
        {
            get { return _luminosity; }
        }

        public HslColor(double hue, double saturation, double luminosity)
            : this(255, hue, saturation, luminosity)
        {
        }

        public HslColor(int alpha, double hue, double saturation, double luminosity)
            : this()
        {
            _alpha = Math.Min(255, Math.Max(0, alpha));
            _hue = CoerceRange(hue);
            _saturation = CoerceRange(saturation);
            _luminosity = CoerceRange(luminosity);
        }

        private static double CoerceRange(double value)
        {
            return Math.Min(1.0, Math.Max(0.0, value));
        }

        public override string ToString()
        {
            return String.Format("[A={3}, H={0:#0.##} S={1:#0.##} L={2:#0.##}]", Hue, Saturation, Luminosity, Alpha);
        }

        public static implicit operator System.Drawing.Color(HslColor hslColor)
        {
            return hslColor.ToColor();
        }

        private static double GetColorComponent(double temp1, double temp2, double temp3)
        {
            temp3 = MoveIntoRange(temp3);

            if (temp3 < 1.0 / 6.0)
                return temp1 + (temp2 - temp1) * 6.0 * temp3;
            if (temp3 < 0.5)
                return temp2;
            if (temp3 < 2.0 / 3.0)
                return temp1 + ((temp2 - temp1) * ((2.0 / 3.0) - temp3) * 6.0);

            return temp1;
        }

        private static double MoveIntoRange(double temp3)
        {
            if (temp3 < 0.0)
                temp3 += 1.0;
            else if (temp3 > 1.0)
                temp3 -= 1.0;

            return temp3;
        }

        private double GetTemp2()
        {
            if (_luminosity < 0.5)  //<=??
                return _luminosity * (1.0 + _saturation);
            else
                return _luminosity + _saturation - (_luminosity * _saturation);
        }

        public static implicit operator HslColor(System.Drawing.Color color)
        {
            return FromColor(color);
        }

        public static HslColor FromColor(System.Drawing.Color color)
        {
            return new HslColor(
                color.A,
                color.GetHue() / 360.0, // we store hue as 0-1 as opposed to 0-360 
                color.GetSaturation(),
                color.GetBrightness()
            );
        }

        public System.Drawing.Color ToColor()
        {
            double r = 0;
            double g = 0;
            double b = 0;

            if (_luminosity != 0)
            {
                if (_saturation == 0)
                {
                    r = g = b = _luminosity;
                }
                else
                {
                    double temp2 = GetTemp2();
                    double temp1 = 2.0 * _luminosity - temp2;

                    r = GetColorComponent(temp1, temp2, _hue + 1.0 / 3.0);
                    g = GetColorComponent(temp1, temp2, _hue);
                    b = GetColorComponent(temp1, temp2, _hue - 1.0 / 3.0);
                }
            }

            return System.Drawing.Color.FromArgb(
                _alpha,
                (int)Math.Round(255 * r),
                (int)Math.Round(255 * g),
                (int)Math.Round(255 * b)
            );
        }
    }
}
