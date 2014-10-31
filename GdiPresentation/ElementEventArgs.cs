using System;
using System.Collections.Generic;
using System.Text;

namespace GdiPresentation
{
    public class ElementEventArgs : EventArgs
    {
        internal bool IsBubblePrevented { get; private set; }

        public void PreventBubble()
        {
            IsBubblePrevented = true;
        }
    }

    public delegate void ElementEventHandler(object sender, ElementEventArgs e);
}
