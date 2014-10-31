using System;
using System.Collections.Generic;
using System.Text;

namespace GdiPresentation
{
    [Flags]
    public enum FontStyle
    {
        Regular = System.Drawing.FontStyle.Regular,
        Bold = System.Drawing.FontStyle.Bold,
        Italic = System.Drawing.FontStyle.Italic,
        Underline = System.Drawing.FontStyle.Underline,
        Strikeout = System.Drawing.FontStyle.Strikeout,
    }
}
