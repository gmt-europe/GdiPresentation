using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GdiPresentation
{
    public class ContainerElement : Element
    {
        public ContainerElement()
        {
            Children = new ElementCollection(this);
            Children.CollectionChanged += Children_CollectionChanged;
        }

        void Children_CollectionChanged(object sender, EventArgs e)
        {
            InvalidateMeasure();
        }

        public ElementCollection Children { get; private set; }

        [DebuggerStepThrough]
        public override int GetChildrenCount()
        {
            return Children.Count;
        }

        [DebuggerStepThrough]
        public override Element GetChild(int index)
        {
            return Children[index];
        }
    }
}
