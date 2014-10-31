using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms.VisualStyles;

namespace GdiPresentation
{
    partial class VisualElement
    {
        private class ListViewHeaderRenderer : IVisualRenderer
        {
            public static readonly ListViewHeaderRenderer Instance = new ListViewHeaderRenderer();

            private static readonly Pen OuterBorder = SystemPens.ControlDarkDark;
            private static readonly Pen LightBorder = SystemPens.ButtonHighlight;
            private static readonly Pen DarkBorder = SystemPens.ButtonShadow;
            private static readonly Pen MediumBorder = SystemPens.ControlLight;
            private static readonly System.Drawing.Brush Face = SystemBrushes.ButtonFace;

            private ListViewHeaderRenderer()
            {
            }

            public VisualStyleElement GetElement(VisualMode mode)
            {
                switch (mode)
                {
                    case VisualMode.Hot: return VisualStyleElement.Header.Item.Hot;
                    case VisualMode.Pressed: return VisualStyleElement.Header.Item.Pressed;
                    default: return VisualStyleElement.Header.Item.Normal;
                }
            }

            public void DrawUnThemed(PaintEventArgs e, Rect bounds, VisualMode mode)
            {
                if (mode == VisualMode.Pressed)
                    DrawPressed(e, bounds);
                else
                    DrawNormal(e, bounds);
            }

            private void DrawNormal(PaintEventArgs e, Rect bounds)
            {
                e.Graphics.DrawLine(
                    LightBorder,
                    bounds.Left,
                    bounds.Top,
                    bounds.Right - 2,
                    bounds.Top
                );
                e.Graphics.DrawLine(
                    LightBorder,
                    bounds.Left,
                    bounds.Top + 1,
                    bounds.Left,
                    bounds.Bottom - 2
                );
                e.Graphics.DrawLine(
                    MediumBorder,
                    bounds.Left + 1,
                    bounds.Top + 1,
                    bounds.Right - 3,
                    bounds.Top + 1
                );
                e.Graphics.DrawLine(
                    MediumBorder,
                    bounds.Left + 1,
                    bounds.Top + 2,
                    bounds.Left + 1,
                    bounds.Bottom - 3
                );
                e.Graphics.DrawLine(
                    OuterBorder,
                    bounds.Right - 1,
                    bounds.Top,
                    bounds.Right - 1,
                    bounds.Bottom - 1
                );
                e.Graphics.DrawLine(
                    OuterBorder,
                    bounds.Left,
                    bounds.Bottom - 1,
                    bounds.Right - 1,
                    bounds.Bottom - 1
                );
                e.Graphics.DrawLine(
                    DarkBorder,
                    bounds.Right - 2,
                    bounds.Top + 1,
                    bounds.Right - 2,
                    bounds.Bottom - 3
                );
                e.Graphics.DrawLine(
                    DarkBorder,
                    bounds.Left + 1,
                    bounds.Bottom - 2,
                    bounds.Right - 2,
                    bounds.Bottom - 2
                );
                e.Graphics.FillRectangle(
                    Face,
                    bounds.Left + 2,
                    bounds.Top + 2,
                    bounds.Width - 4,
                    bounds.Height - 4
                );
            }

            private void DrawPressed(PaintEventArgs e, Rect bounds)
            {
                e.Graphics.DrawRectangle(
                    DarkBorder,
                    bounds.Left,
                    bounds.Top,
                    bounds.Width - 1,
                    bounds.Height - 1
                );
                e.Graphics.FillRectangle(
                    Face,
                    bounds.Left + 1,
                    bounds.Top + 1,
                    bounds.Width - 2,
                    bounds.Height - 2
                );
            }
        }
    }
}
