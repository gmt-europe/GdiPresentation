using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace GdiPresentation.Demo
{
    public partial class FocusDemoControl : UserControl
    {
        public FocusDemoControl()
        {
            InitializeComponent();
        }

        private void FocusDemoControl_Load(object sender, EventArgs e)
        {
            var demo = new KeyboardFocusDemo();

            demo.PrepareControl(_control);
            _control.Content = demo.BuildContent();
        }
    }
}
