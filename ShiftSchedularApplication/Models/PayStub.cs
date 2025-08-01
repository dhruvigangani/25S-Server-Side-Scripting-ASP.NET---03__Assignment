using System;
using System.ComponentModel.DataAnnotations;

namespace ShiftSchedularApplication.Models
{
    public class PayStub
    {
        public int Id { get; set; }

        [Required]
        public string EmployeeId { get; set; } = string.Empty;  // FK

        [Required]
        public decimal HoursWorked { get; set; }

        [Required]
        public decimal HourlyRate { get; set; }

        public decimal TotalPay => HoursWorked * HourlyRate;

        [Required]
        public DateTime PayDate { get; set; }
    }
}