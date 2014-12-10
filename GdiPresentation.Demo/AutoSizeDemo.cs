using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

namespace GdiPresentation.Demo
{
    [DisplayName("Auto Size")]
    internal class AutoSizeDemo : Demo
    {
        public override Control CreateControl()
        {
            return new AutoSizeDemoControl();
        }
    }
}
