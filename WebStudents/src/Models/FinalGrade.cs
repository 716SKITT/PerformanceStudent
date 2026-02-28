using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentsPerformance.Models;

public class FinalGrade
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid GradeSheetId { get; set; }

    [Required]
    public Guid StudentId { get; set; }

    [ForeignKey(nameof(GradeSheetId))]
    public GradeSheet? GradeSheet { get; set; }

    [ForeignKey(nameof(StudentId))]
    public Student? Student { get; set; }

    public decimal? FinalScore { get; set; }

    [MaxLength(20)]
    public string? FinalMark { get; set; }

    public DateTime UpdatedAt { get; set; }
}
