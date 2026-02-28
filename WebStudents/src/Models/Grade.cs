using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentsPerformance.Models;

public class Grade
{
    [Key]
    public int Id { get; set; }

    [Required]
    public Guid StudentId { get; set; }

    [ForeignKey("StudentId")]
    public Student? Student { get; set; }

    [Required]
    public int AssignmentId { get; set; }

    [ForeignKey("AssignmentId")]
    public Assignment Assignment { get; set; } = null!;

    [Required]
    [Range(0, 100)]
    public double Score { get; set; }

    public Guid? DisciplineOfferingId { get; set; }

    [ForeignKey(nameof(DisciplineOfferingId))]
    public DisciplineOffering? DisciplineOffering { get; set; }
}
