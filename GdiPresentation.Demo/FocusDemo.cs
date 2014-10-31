using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

namespace GdiPresentation.Demo
{
    [DisplayName("Focus")]
    internal class FocusDemo : Demo
    {
        public override Control CreateControl()
        {
            return new FocusDemoControl();
        }
    }
}
