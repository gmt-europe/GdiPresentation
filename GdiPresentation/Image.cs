using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GdiPresentation
{
    public class Image : Element
    {
        private System.Drawing.Image _bitmap;
        private Stretch _stretch;
        private StretchDirection _stretchDirection;

        public System.Drawing.Image Bitmap
        {
            get { return _bitmap; }
            set
            {
                if (_bitmap != value)
                {
                    _bitmap = value;
                    Invalidate();
                }
            }
        }

        public Stretch Stretch
        {
            get { return _stretch; }
            set
            {
                if (_stretch != value)
                {
                    _stretch = value;
                    Invalidate();
                }
            }
        }

        public StretchDirection StretchDirection
        {
            get { return _stretchDirection; }
            set
            {
                if (_stretchDirection != value)
                {
                    _stretchDirection = value;
                    Invalidate();
                }
            }
        }

        public Image()
        {
            _stretch = Stretch.Uniform;
            _stretchDirection = StretchDirection.Both;
        }

        protected override Size MeasureOverride(Size desiredSize)
        {
            if (_bitmap == null)
                return Size.Empty;
            else
                return (Size)_bitmap.Size;
        }

        protected override void OnPaint(ElementPaintEventArgs e)
        {
            Debug.Assert(LayoutManager != null);

            if (LayoutManager == null)
                return;

            if (_bitmap != null)
            {
                var size = (Size)_bitmap.Size;
                var targetSize = size;

                switch (Stretch)
                {
                    case Stretch.Fill:
                        targetSize = e.Bounds.Size;
                        break;

                    case Stretch.None:
                        targetSize = size;
                        break;

                    default:
                        double heightFactor = (double)e.Bounds.Height / size.Height;
                        double widthFactor = (double)e.Bounds.Width / size.Width;

                        bool useHeight = heightFactor > widthFactor;

                        if (Stretch == Stretch.Uniform)
                            useHeight = !useHeight;

                        if (useHeight)
                            targetSize = new Size((int)(size.Width * heightFactor), e.Bounds.Height);
                        else
                            targetSize = new Size(e.Bounds.Width, (int)(size.Height * widthFactor));
                        break;
                }

                switch (StretchDirection)
                {
                    case StretchDirection.DownOnly:
                        targetSize = new Size(
                            Math.Max(targetSize.Width, size.Width),
                            Math.Max(targetSize.Height, size.Height)
                        );
                        break;

                    case StretchDirection.UpOnly:
                        targetSize = new Size(
                            Math.Min(targetSize.Width, size.Width),
                            Math.Min(targetSize.Height, size.Height)
                        );
                        break;
                }

                var clip = e.Graphics.Clip;

                try
                {
                    e.Graphics.SetClip((System.Drawing.Rectangle)e.Bounds);

                    e.Graphics.DrawImage(_bitmap, new System.Drawing.Rectangle(
                        e.Bounds.Left + Math.Max((e.Bounds.Width - targetSize.Width) / 2, 0),
                        e.Bounds.Top + Math.Max((e.Bounds.Height - targetSize.Height) / 2, 0),
                        targetSize.Width,
                        targetSize.Height
                    ));
                }
                finally
                {
                    e.Graphics.Clip = clip;
                }
            }

            base.OnPaint(e);
        }
    }
}
