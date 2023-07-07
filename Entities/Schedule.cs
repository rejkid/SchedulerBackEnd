using System;

namespace WebApi.Entities
{
    public class Schedule
    {
        public int ScheduleId { get; set; }
        public DateTime Date { get; set; }
        public Boolean Required { get; set; }
        public Boolean UserAvailability { get; set; }
        public string UserFunction { get; set; }
        // Notification section
        public bool NotifiedWeekBefore { get; set; } = false;
        public bool NotifiedThreeDaysBefore { get; set; } = false;
        // End of Notification section

        /* Reward section*/
        public uint NoOfTimesAssigned { get; set; }
        public uint NoOfTimesDropped { get; set; }
    }
}