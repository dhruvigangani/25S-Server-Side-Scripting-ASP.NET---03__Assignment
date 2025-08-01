using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;


namespace ShiftSchedularApplication.Models
{
    public class Availability
    {
        public int Id { get; set; }

        [Required]
        public string EmployeeId { get; set; } = string.Empty;  // FK to AspNetUsers.Id

        [Required]
        public DayOfWeek Day { get; set; }

        [Required]
        public TimeSpan StartAvailability { get; set; }

        [Required]
        public TimeSpan EndAvailability { get; set; }
    }
}