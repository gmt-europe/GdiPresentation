using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GdiPresentation.Demo
{
    public partial class AutoSizeDemoControl : UserControl
    {
        public AutoSizeDemoControl()
        {
            InitializeComponent();
        }

        private void AutoSizeDemoControl_Load(object sender, EventArgs e)
        {
            var demo = new ChangeVisibilityDemo();

            demo.PrepareControl(_control);
            _control.Content = demo.BuildContent();
        }
    }
}
