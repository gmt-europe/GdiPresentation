using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

namespace GdiPresentation
{
    public class Border : ContentElement
    {
        private Brush _borderBrush;
        private Thickness _padding;
        private Thickness _borderThickness;
        private CornerRadius _cornerRadius;

        public Brush BorderBrush
        {
            get { return _borderBrush; }
            set
            {
                if (_borderBrush != value)
                {
                    _borderBrush = value;
                    Invalidate();
                }
            }
        }

        public Thickness BorderThickness
        {
            get { return _borderThickness; }
            set
            {
                if (_borderThickness != value)
                {
                    _borderThickness = value;
                    InvalidateMeasure();
                }
            }
        }

        public CornerRadius CornerRadius
        {
            get { return _cornerRadius; }
            set
            {
                if (_cornerRadius != value)
                {
                    _cornerRadius = value;
                    Invalidate();
                }
            }
        }

        public Thickness Padding
        {
            get { return _padding; }
            set
            {
                if (_padding != value)
                {
                    _padding = value;
                    InvalidateMeasure();
                }
            }
        }

        protected override Size MeasureOverride(Size desiredSize)
        {
            var padding = new Size(
                Padding.Horizontal + _borderThickness.Horizontal,
                Padding.Vertical + _borderThickness.Vertical
            );

            desiredSize = base.MeasureOverride(new Size(
                desiredSize.Width - padding.Width,
                desiredSize.Height - padding.Height
            ));

            return new Size(
                desiredSize.Width + padding.Width,
                desiredSize.Height + padding.Height
            );
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Content != null)
            {
                Content.Arrange(new Rect(
                    Padding.Left + _borderThickness.Left,
                    Padding.Top + _borderThickness.Top,
                    finalSize.Width - (Padding.Horizontal + _borderThickness.Horizontal),
                    finalSize.Height - (Padding.Vertical + _borderThickness.Vertical)
                ));

                return Content.Size;
            }
            else
            {
                return base.ArrangeOverride(finalSize);
            }
        }

        protected override void OnPaintBackground(ElementPaintEventArgs e)
        {
            if (Background == null || Background == Brush.Transparent)
                return;

            using (var brush = Background.CreateBrush(e.Bounds))
            {
                if (_cornerRadius.IsEmpty)
                {
                    e.Graphics.FillRectangle(brush, (Rectangle)e.Bounds);
                }
                else
                {
                    using (var path = GetRoundedRect(e.Bounds))
                    using (var region = new Region(path))
                    {
                        region.Intersect((Rectangle)e.Bounds);

                        var smoothingMode = e.Graphics.SmoothingMode;

                        try
                        {
                            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                            e.Graphics.FillRegion(brush, region);
                        }
                        finally
                        {
                            e.Graphics.SmoothingMode = smoothingMode;
                        }
                    }
                }
            }
        }

        protected override void OnPaint(ElementPaintEventArgs e)
        {
            Debug.Assert(LayoutManager != null);

            if (LayoutManager == null)
                return;

            if (_borderBrush == null || _borderThickness.IsEmpty)
                return;

            using (var brush = _borderBrush.CreateBrush(e.Bounds))
            {
                // Simple case for a single rectangle.

                if (
                    _borderThickness.Left == _borderThickness.Top &&
                    _borderThickness.Left == _borderThickness.Right &&
                    _borderThickness.Left == _borderThickness.Bottom
                )
                {
                    var bounds = new Rect(
                        e.Bounds.Left + _borderThickness.Left / 2,
                        e.Bounds.Top + _borderThickness.Left / 2,
                        e.Bounds.Width - _borderThickness.Left,
                        e.Bounds.Height - _borderThickness.Left
                    );

                    using (var pen = new Pen(brush, _borderThickness.Left))
                    {
                        if (_cornerRadius.IsEmpty)
                        {
                            e.Graphics.DrawRectangle(pen, (Rectangle)bounds);
                        }
                        else
                        {
                            using (var path = GetRoundedRect(bounds))
                            {
                                var smoothingMode = e.Graphics.SmoothingMode;

                                try
                                {
                                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                                    e.Graphics.DrawPath(pen, path);
                                }
                                finally
                                {
                                    e.Graphics.SmoothingMode = smoothingMode;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (!_cornerRadius.IsEmpty)
                        throw new InvalidOperationException("Invalid corner radius for thickness");

                    // Otherwise, we need to do a little bit more complex
                    // calculations.

                    using (var region = new Region((Rectangle)e.Bounds))
                    {
                        region.Exclude(new Rectangle(
                            e.Bounds.Left + _borderThickness.Left,
                            e.Bounds.Top + _borderThickness.Top,
                            e.Bounds.Width - _borderThickness.Horizontal,
                            e.Bounds.Height - _borderThickness.Vertical
                            ));

                        e.Graphics.FillRegion(brush, region);
                    }
                }
            }

            base.OnPaint(e);
        }

        private GraphicsPath GetRoundedRect(Rect bounds)
        {
            // If the corner radius is greater than or equal to 
            // half the width, or height (whichever is shorter) 
            // then return a capsule instead of a lozenge 

            if (
                Math.Max(Math.Max(_cornerRadius.TopLeft, _cornerRadius.TopRight), Math.Max(_cornerRadius.BottomLeft, _cornerRadius.BottomRight)) >=
                Math.Min(bounds.Width, bounds.Height) / 2
            )
                return GetCapsule(bounds);

            var path = new GraphicsPath();

            // Top left.

            int topLeft = _cornerRadius.TopLeft * 2;

            path.AddArc(
                bounds.X,
                bounds.Y,
                topLeft,
                topLeft,
                180,
                90
            );

            // Top right.

            int topRight = _cornerRadius.TopRight * 2;

            path.AddArc(
                bounds.Right - topRight,
                bounds.Y,
                topRight,
                topRight,
                270,
                90
            );

            // Bottom right.

            int bottomRight = _cornerRadius.BottomRight * 2;

            path.AddArc(
                bounds.Right - bottomRight,
                bounds.Bottom - bottomRight,
                bottomRight,
                bottomRight,
                0,
                90
            );

            // Bottom left.

            int bottomLeft = _cornerRadius.BottomLeft * 2;

            path.AddArc(
                bounds.X,
                bounds.Bottom - bottomLeft,
                bottomLeft,
                bottomLeft,
                90,
                90
            );

            path.CloseFigure();

            return path;
        }

        private GraphicsPath GetCapsule(Rect bounds)
        {
            var path = new GraphicsPath();
            bool success = true;

            try
            {
                if (bounds.Width > bounds.Height)
                {
                    // Horizontal capsule.

                    path.AddArc(
                        bounds.X,
                        bounds.Y,
                        bounds.Height,
                        bounds.Height,
                        90,
                        180
                    );

                    path.AddArc(
                        bounds.Right - bounds.Height,
                        bounds.Y,
                        bounds.Height,
                        bounds.Height,
                        270,
                        180
                    );
                }
                else if (bounds.Width < bounds.Height)
                {
                    // Vertical capsule.

                    path.AddArc(
                        bounds.X,
                        bounds.Y,
                        bounds.Width,
                        bounds.Width,
                        180,
                        180
                    );

                    path.AddArc(
                        bounds.X,
                        bounds.Bottom - bounds.Width,
                        bounds.Width,
                        bounds.Width,
                        0,
                        180
                    );
                }
                else
                {
                    success = false;
                }
            }
            catch
            {
                success = false;
            }

            if (!success)
                path.AddEllipse((Rectangle)bounds);

            path.CloseFigure();

            return path;
        }
    }
}
