using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace GdiPresentation
{
    public static class ElementStatistics
    {
        private static readonly object _syncRoot = new object();
        private static List<ElementStatisticsEvent> _events = new List<ElementStatisticsEvent>();

        public static bool IsEnabled { get; set; }

        internal static void AddEvent(ElementStatisticsEventType type, TimeSpan duration)
        {
            Debug.Assert(IsEnabled);

            lock (_syncRoot)
            {
                _events.Add(new ElementStatisticsEvent(type, duration));
            }
        }

        public static ElementStatisticsEvent[] GetNewEvents()
        {
            lock (_syncRoot)
            {
                var result = _events.ToArray();

                _events = new List<ElementStatisticsEvent>();

                return result;
            }
        }
    }
}
