using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GdiPresentation
{
    public class PaintEventArgs
    {
        public Graphics Graphics { get; private set; }

        public Rect ClipRectangle { get; private set; }

        public PaintEventArgs(Graphics graphics, Rect clipRectangle)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");

            Graphics = graphics;
            ClipRectangle = clipRectangle;
        }
    }

    public delegate void PaintEventHandler(object sender, PaintEventArgs e);
}
