using System;
using System.Collections.Generic;
using System.Text;

namespace GdiPresentation
{
    internal class ResolveCursorEventArgs : MouseEventArgs
    {
        public ResolveCursorEventArgs(MouseButtons button, int clicks, int delta)
            : base(button, clicks, delta)
        {
        }

        public Cursor Cursor { get; set; }
    }
}
