using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentsPerformance.Models;

public class GradeSheet
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid DisciplineOfferingId { get; set; }

    [ForeignKey(nameof(DisciplineOfferingId))]
    public DisciplineOffering? DisciplineOffering { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public SheetStatus Status { get; set; }

    public ICollection<FinalGrade> FinalGrades { get; set; } = new List<FinalGrade>();
}
