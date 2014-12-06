using System;
using System.Collections.Generic;
using System.Text;

namespace GdiPresentation
{
    public class PreferredSizeChangedEventArgs : EventArgs
    {
        public Size Size { get; private set; }

        public PreferredSizeChangedEventArgs(Size size)
        {
            Size = size;
        }
    }

    public delegate void PreferredSizeChangedEventHandler(object sender, PreferredSizeChangedEventArgs e);
}
