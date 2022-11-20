using System;

namespace WebApi.Entities
{
    public class Schedule
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public Boolean Required { get; set; }
        public Boolean UserAvailability { get; set; }
        public string UserFunction { get; set; }
    }
}