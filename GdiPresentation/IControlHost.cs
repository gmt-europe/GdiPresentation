using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace GdiPresentation
{
    internal interface IControlHost
    {
        Control Control { get; }
    }
}
