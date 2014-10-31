using System;
using System.Collections.Generic;
using System.Text;

namespace GdiPresentation
{
    public class MouseEventArgs : ElementEventArgs
    {
        private readonly Point _location;

        public MouseButtons Button { get; private set; }

        public int Clicks { get; private set; }

        public int Delta { get; private set; }

        internal bool FocusChanged { get; set; }

        internal bool IsMouseDirectlyOverAssigned { get; set; }

        public MouseEventArgs(MouseButtons button, int clicks, int delta)
            : this((Point)System.Windows.Forms.Cursor.Position, button, clicks, delta)
        {
        }

        internal MouseEventArgs(Point location, MouseButtons button, int clicks, int delta)
        {
            _location = location;
            Button = button;
            Clicks = clicks;
            Delta = delta;
        }

        public Point GetPosition(Element relativeTo)
        {
            if (relativeTo == null)
                throw new ArgumentNullException("relativeTo");

            return relativeTo.TranslateLocation(_location);
        }
    }

    public delegate void MouseEventHandler(object sender, MouseEventArgs e);
}
