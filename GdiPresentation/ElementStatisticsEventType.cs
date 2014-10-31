using System;
using System.Collections.Generic;
using System.Text;

namespace GdiPresentation
{
    [Flags]
    public enum ElementStatisticsEventType
    {
        Measure = 1,
        Arrange = 2,
        Paint = 4,
        Forced = 8
    }
}
