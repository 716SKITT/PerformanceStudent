using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentsPerformance.Models;

public class Attendance
{
    [Key]
    public int Id { get; set; }

    [Required]
    public Guid StudentId { get; set; }

    [ForeignKey("StudentId")]
    public Student? Student { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public bool IsPresent { get; set; }
}
