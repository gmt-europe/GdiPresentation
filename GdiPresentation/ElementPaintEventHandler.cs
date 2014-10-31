using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GdiPresentation
{
    public class ElementPaintEventArgs : PaintEventArgs
    {
        public Rect Bounds { get; private set; }

        public ElementPaintEventArgs(Graphics graphics, Rect clipRectangle, Rect bounds)
            : base(graphics, clipRectangle)
        {
            Bounds = bounds;
        }
    }

    public delegate void ElementPaintEventHandler(object sender, ElementPaintEventArgs e);
}
