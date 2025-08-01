using System;
using System.ComponentModel.DataAnnotations;

namespace ShiftSchedularApplication.Models
{
    public class Punch
    {
        public int Id { get; set; }

        [Required]
        public string EmployeeId { get; set; } = string.Empty;  // FK

        [Required]
        public DateTime PunchInTime { get; set; }

        public DateTime? PunchOutTime { get; set; }
    }
}