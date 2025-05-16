using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentsPerformance.Models;

public class Assignment
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public DateTime DueDate { get; set; }

    [Required]
    public int CourseId { get; set; }

    [ForeignKey("CourseId")]
    public Course? Course { get; set; }
}
