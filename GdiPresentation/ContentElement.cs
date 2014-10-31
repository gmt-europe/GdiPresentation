using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GdiPresentation
{
    public abstract class ContentElement : Element
    {
        private Element _content;

        public Element Content
        {
            get { return _content; }
            set
            {
                if (_content != value)
                {
                    if (_content != null)
                        _content.Parent = null;

                    _content = value;

                    if (_content != null)
                        _content.Parent = this;

                    InvalidateMeasure();
                }
            }
        }

        protected override Size MeasureOverride(Size desiredSize)
        {
            if (Content != null)
            {
                Content.Measure(desiredSize);

                return Content.DesiredSize;
            }
            else
            {
                return base.MeasureOverride(desiredSize);
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Content != null)
            {
                Content.Arrange(new Rect(Point.Empty, finalSize));

                return Content.Size;
            }
            else
            {
                return base.ArrangeOverride(finalSize);
            }
        }

        [DebuggerStepThrough]
        public override int GetChildrenCount()
        {
            return Content == null ? 0 : 1;
        }

        [DebuggerStepThrough]
        public override Element GetChild(int index)
        {
            return Content;
        }
    }
}
