using System;
using System.ComponentModel.DataAnnotations;

namespace ShiftScheduler.Models
{
    public class PayStub
    {
        public int Id { get; set; }

        [Required]
        public string EmployeeId { get; set; }

        [Required]
        public decimal HoursWorked { get; set; }

        [Required]
        public decimal HourlyRate { get; set; }

        public decimal TotalPay => HoursWorked * HourlyRate;

        public DateTime PayDate { get; set; }
    }
}