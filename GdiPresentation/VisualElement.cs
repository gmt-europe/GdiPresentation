using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace GdiPresentation
{
    public partial class VisualElement : ContentElement
    {
        private VisualStyle _style;
        private VisualMode _mode;

        public VisualStyle Style
        {
            get { return _style; }
            set
            {
                if (_style != value)
                {
                    _style = value;
                    Invalidate();
                }
            }
        }

        public VisualMode Mode
        {
            get { return _mode; }
            set
            {
                if (_mode != value)
                {
                    _mode = value;
                    Invalidate();
                }
            }
        }

        protected override void OnPaintBackground(ElementPaintEventArgs e)
        {
            var renderer = GetRenderer();
            var element = renderer.GetElement(Mode);

            if (Application.RenderWithVisualStyles && VisualStyleRenderer.IsElementDefined(element))
                new VisualStyleRenderer(element).DrawBackground(e.Graphics, (Rectangle)e.Bounds);
            else
                renderer.DrawUnThemed(e, e.Bounds, Mode);
        }

        private IVisualRenderer GetRenderer()
        {
            switch (Style)
            {
                case VisualStyle.ListViewHeader: return ListViewHeaderRenderer.Instance;
                default: throw new InvalidOperationException("Style is invalid");
            }
        }

        private interface IVisualRenderer
        {
            VisualStyleElement GetElement(VisualMode mode);

            void DrawUnThemed(PaintEventArgs e, Rect bounds, VisualMode mode);
        }
    }
}
