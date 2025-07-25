using System;
using System.ComponentModel.DataAnnotations;

namespace ShiftScheduler.Models
{
    public class Punch
    {
        public int Id { get; set; }

        [Required]
        public string EmployeeId { get; set; }

        [Required]
        public DateTime PunchInTime { get; set; }

        public DateTime? PunchOutTime { get; set; }
    }
}