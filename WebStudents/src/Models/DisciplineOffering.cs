using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentsPerformance.Models;

public class DisciplineOffering
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid DisciplineId { get; set; }

    [Required]
    public Guid SemesterId { get; set; }

    [Required]
    public Guid StudentGroupId { get; set; }

    [Required]
    public Guid ProffessorId { get; set; }

    [ForeignKey(nameof(DisciplineId))]
    public Discipline? Discipline { get; set; }

    [ForeignKey(nameof(SemesterId))]
    public Semester? Semester { get; set; }

    [ForeignKey(nameof(StudentGroupId))]
    public StudentGroup? StudentGroup { get; set; }

    [ForeignKey(nameof(ProffessorId))]
    public Proffessor? Proffessor { get; set; }

    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    public ICollection<Grade> Grades { get; set; } = new List<Grade>();
    public ICollection<GradeSheet> GradeSheets { get; set; } = new List<GradeSheet>();
}
