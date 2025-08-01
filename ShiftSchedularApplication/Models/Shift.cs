using System;
using System.ComponentModel.DataAnnotations;

namespace ShiftSchedularApplication.Models
{
    public class Shift
    {
        public int Id { get; set; }

        [Required]
        public string EmployeeId { get; set; } = string.Empty;  // FK

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public bool IsSwapRequested { get; set; }
        public bool IsGivenAway { get; set; }
        public bool IsAbsent { get; set; }
    }
}