using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentsPerformance.Models;

public class StudentGroup
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(1, 6)]
    public int StudyYear { get; set; }

    [Required]
    public Guid AcademicYearId { get; set; }

    [ForeignKey(nameof(AcademicYearId))]
    public AcademicYear? AcademicYear { get; set; }

    public ICollection<Student> Students { get; set; } = new List<Student>();
    public ICollection<DisciplineOffering> DisciplineOfferings { get; set; } = new List<DisciplineOffering>();
}
