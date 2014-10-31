using System;
using System.Collections.Generic;
using System.Text;

namespace GdiPresentation
{
    public class HeaderedContainerElement : ContainerElement
    {
        private Element _header;

        public Element Header
        {
            get { return _header; }
            set
            {
                if (_header != value)
                {
                    if (_header != null)
                        _header.Parent = null;

                    _header = value;

                    if (_header != null)
                        _header.Parent = this;

                    InvalidateMeasure();
                }
            }
        }

        public override int GetChildrenCount()
        {
            int children = base.GetChildrenCount();

            if (Header != null)
                children++;

            return children;
        }

        public override Element GetChild(int index)
        {
            if (Header != null)
            {
                if (index == 0)
                    return Header;

                index--;
            }

            return base.GetChild(index);
        }
    }
}
