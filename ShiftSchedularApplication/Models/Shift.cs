using System.ComponentModel.DataAnnotations;

public class Shift
{
    public int Id { get; set; }

    [Required]
    public string EmployeeId { get; set; }  // FK to Identity User

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    public bool IsSwapRequested { get; set; }
    public bool IsGivenAway { get; set; }
    public bool IsAbsent { get; set; }
}