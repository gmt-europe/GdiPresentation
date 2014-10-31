using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace GdiPresentation.Demo
{
    internal abstract class Demo : IDisposable
    {
        public abstract Control CreateControl();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
