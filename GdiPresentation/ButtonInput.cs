using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace GdiPresentation
{
    public class ButtonInput : ControlHost<Button>
    {
        public ButtonInput()
            : base(new Button())
        {
            Control.FlatStyle = FlatStyle.System;
        }

        public string Text
        {
            get { return Control.Text; }
            set { Control.Text = value; }
        }
    }
}
