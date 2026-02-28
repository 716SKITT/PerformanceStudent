using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentsPerformance.Models;

public class Semester
{
    [Key]
    public Guid Id { get; set; }

    [Range(1, 2)]
    public int Number { get; set; }

    [Required]
    public Guid AcademicYearId { get; set; }

    [ForeignKey(nameof(AcademicYearId))]
    public AcademicYear? AcademicYear { get; set; }

    public ICollection<DisciplineOffering> DisciplineOfferings { get; set; } = new List<DisciplineOffering>();
}
