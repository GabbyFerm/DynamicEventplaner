using System;

namespace DynamicEventplaner.Classes
{
    public class Event
    {
        public enum RecurrencePattern
        {
            Daily,
            Weekly,
            Monthly
        }

        public required string Name { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public required string TimeZone { get; set; }
        public bool IsRecurring { get; set; }
        public RecurrencePattern Pattern { get; set; }

        public DateTime GetEndTime()
        {
            return StartTime.Add(Duration);
        }
    }
}