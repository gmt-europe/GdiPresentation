using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GdiPresentation
{
    public class Link : Run
    {
        public Uri Target { get; set; }

        public event EventHandler Click;

        protected virtual void OnClick(EventArgs e)
        {
            var ev = Click;

            if (ev != null)
                ev(this, e);
        }

        public Link()
        {
            FontStyle = GdiPresentation.FontStyle.Underline;
            ForeColor = Color.Blue;
            Cursor = Cursor.Hand;
        }

        public Link(string text)
            : base(text)
        {
        }

        public Link(string text, Uri target)
            : this(text)
        {
            Target = target;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            e.PreventBubble();

            if (e.Button == MouseButtons.Left)
                Capture = true;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            e.PreventBubble();

            if (Capture && IsMouseDirectlyOver)
            {
                if (Target != null)
                    SEH.SinkExceptions(() => Process.Start(Target.ToString()));

                OnClick(EventArgs.Empty);
            }

            Capture = false;
        }
    }
}
