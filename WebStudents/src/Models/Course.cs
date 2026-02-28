using System.ComponentModel.DataAnnotations;

namespace StudentsPerformance.Models;

public class Course
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Student> Students { get; set; } = new List<Student>();
}
