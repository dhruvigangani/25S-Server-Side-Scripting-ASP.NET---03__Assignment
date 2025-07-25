using System;
using System.ComponentModel.DataAnnotations;

namespace ShiftScheduler.Models
{
    public class Availability
    {
        public int Id { get; set; }

        [Required]
        public string EmployeeId { get; set; }

        [Required]
        public DayOfWeek Day { get; set; }

        public TimeSpan StartAvailability { get; set; }
        public TimeSpan EndAvailability { get; set; }


    }
}