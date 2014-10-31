using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace GdiPresentation.Demo
{
    internal abstract class ElementDemo : Demo
    {
        public override Control CreateControl()
        {
            var control = new ElementControl
            {
                AllowedScrollBars = AllowedScrollBars,
                ShowHorizontalScrollBar = ShowHorizontalScrollBar,
                ShowVerticalScrollBar = ShowVerticalScrollBar,
                Dock = DockStyle.Fill
            };

            PrepareControl(control);

            control.Content = BuildContent();

            return control;
        }

        public abstract Element BuildContent();

        public virtual void PrepareControl(ElementControl control)
        {
        }

        public abstract ScrollBars AllowedScrollBars { get; }

        public virtual ScrollBarVisibility ShowHorizontalScrollBar
        {
            get { return ScrollBarVisibility.Auto; }
        }

        public virtual ScrollBarVisibility ShowVerticalScrollBar
        {
            get { return ScrollBarVisibility.Auto; }
        }
    }
}
