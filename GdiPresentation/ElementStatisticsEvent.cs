using System;
using System.Collections.Generic;
using System.Text;

namespace GdiPresentation
{
    public class ElementStatisticsEvent
    {
        public ElementStatisticsEventType Type { get; private set; }
        public TimeSpan Duration { get; private set; }

        public ElementStatisticsEvent(ElementStatisticsEventType type, TimeSpan duration)
        {
            Type = type;
            Duration = duration;
        }
    }
}
