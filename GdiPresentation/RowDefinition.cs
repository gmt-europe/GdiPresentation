using System;
using System.Collections.Generic;
using System.Text;

namespace GdiPresentation
{
    public class RowDefinition : DefinitionBase
    {
        private int _maxHeight = int.MaxValue;
        private int _minHeight;
        private GridLength _height;

        public int ActualHeight { get; internal set; }

        public int MaxHeight
        {
            get { return _maxHeight; }
            set
            {
                if (_maxHeight != value)
                {
                    _maxHeight = value;
                    if (Parent != null)
                        Parent.InvalidateArrange();
                }
            }
        }

        public int MinHeight
        {
            get { return _minHeight; }
            set
            {
                if (_minHeight != value)
                {
                    _minHeight = value;
                    if (Parent != null)
                        Parent.InvalidateArrange();
                }
            }
        }

        public GridLength Height
        {
            get { return _height; }
            set
            {
                if (_height != value)
                {
                    _height = value;
                    if (Parent != null)
                        Parent.InvalidateArrange();
                }
            }
        }

        public RowDefinition()
        {
        }

        public RowDefinition(GridLength height)
        {
            Height = height;
        }
    }
}
