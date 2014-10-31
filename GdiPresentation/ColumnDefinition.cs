using System;
using System.Collections.Generic;
using System.Text;

namespace GdiPresentation
{
    public class ColumnDefinition : DefinitionBase
    {
        private int _maxWidth = int.MaxValue;
        private int _minWidth;
        private GridLength _width;

        public int ActualWidth { get; internal set; }

        public int MaxWidth
        {
            get { return _maxWidth; }
            set
            {
                if (_maxWidth != value)
                {
                    _maxWidth = value;
                    if (Parent != null)
                        Parent.InvalidateArrange();
                }
            }
        }

        public int MinWidth
        {
            get { return _minWidth; }
            set
            {
                if (_minWidth != value)
                {
                    _minWidth = value;
                    if (Parent != null)
                        Parent.InvalidateArrange();
                }
            }
        }

        public GridLength Width
        {
            get { return _width; }
            set
            {
                if (_width != value)
                {
                    _width = value;
                    if (Parent != null)
                        Parent.InvalidateArrange();
                }
            }
        }

        public ColumnDefinition()
        {
        }

        public ColumnDefinition(GridLength width)
        {
            Width = width;
        }
    }
}
