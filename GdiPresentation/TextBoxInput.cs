using System;
using System.Collections.Generic;
using System.Text;

namespace GdiPresentation
{
    public class TextBoxInput : ControlHost<System.Windows.Forms.TextBox>
    {
        public TextBoxInput()
            : base(new System.Windows.Forms.TextBox())
        {
        }
    }
}
